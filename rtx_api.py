from flask import Flask, request, jsonify
import requests
import random
import string
import psutil
import json
import os

app = Flask(__name__)

port = None
fn_index = None

appdata_folder = os.path.dirname(os.getenv('APPDATA')).replace('\\', '/')
cert_path = appdata_folder + "/Local/NVIDIA/ChatRTX/RAG/trt-llm-rag-windows-ChatRTX_0.3/certs/servercert.pem"
key_path = appdata_folder + "/Local/NVIDIA/ChatRTX/RAG/trt-llm-rag-windows-ChatRTX_0.3/certs/serverkey.pem"
ca_bundle = appdata_folder + "/Local/NVIDIA/ChatRTX/env_nvd_rag/Library/ssl/cacert.pem"

def find_chat_with_rtx_port():
    global port
    connections = psutil.net_connections(kind='inet')
    for host in connections:
        try:
            if host.pid:
                process = psutil.Process(host.pid)
                if "chatrtx" in process.exe().lower():
                    test_port = host.laddr.port
                    url = f"https://127.0.0.1:{test_port}/queue/join"
                    response = requests.post(url, data="", timeout=2, cert=(cert_path, key_path), verify=ca_bundle)
                    if response.status_code == 422:
                        port = test_port
                        return
        except:
            pass

def join_queue(session_hash, set_fn_index, port, chatdata):
    python_object = {
        "data": chatdata,
        "event_data": None,
        "fn_index": set_fn_index,
        "session_hash": session_hash
    }
    json_string = json.dumps(python_object)
    url = f"https://127.0.0.1:{port}/queue/join"
    response = requests.post(url, data=json_string, cert=(cert_path, key_path), verify=ca_bundle)
    print("Join Queue Response:", response)

def listen_for_updates(session_hash, port):
    url = f"https://127.0.0.1:{port}/queue/data?session_hash={session_hash}"
    response = requests.get(url, stream=True, cert=(cert_path, key_path), verify=ca_bundle)
    print("Listen Response:", response)
    try:
        for line in response.iter_lines():
            if line:
                data = json.loads(line[5:])
                if data['msg'] == 'process_completed':
                    return data['output']['data'][0][0][1]
    except Exception as e:
        pass
    return ""

def auto_find_fn_index(port):
    global fn_index
    print("Searching for llm streamed completion function. Takes about 30 seconds.")
    session_hash = ''.join(random.choices(string.ascii_letters + string.digits, k=10))
    chatdata = [[["write a comma", None]], None]
    for i in range(10, 1000):
        join_queue(session_hash, i, port, chatdata)
        res = listen_for_updates(session_hash, port)
        if res:
            fn_index = i
            return
    raise Exception("Failed to find fn_index")

def send_message(message):
    global fn_index
    if not port:
        find_chat_with_rtx_port()
    if not port:
        raise Exception("Failed to find a server port for 'Chat with RTX'. Ensure the server is running.")
    if not fn_index:
        auto_find_fn_index(port)
        print("To make initialization instant hardcode:\nfn_index =", fn_index)
    session_hash = ''.join(random.choices(string.ascii_letters + string.digits, k=10))
    chatdata = [[[message, None]], None]
    join_queue(session_hash, fn_index, port, chatdata)
    response = listen_for_updates(session_hash, port)
    save_chat_data(response)
    return response

def save_chat_data(chat_data, file_path="chat_history.txt"):
    with open(file_path, 'a', encoding='utf-8') as file:
        file.write(chat_data + '\n')

@app.route('/send_message', methods=['POST'])
def handle_send_message():
    data = request.json
    message = data.get('message', '')
    response = send_message(message)
    return jsonify({'response': response})

if __name__ == '__main__':
    app.run(port=5000)
