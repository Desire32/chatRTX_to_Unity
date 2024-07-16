import requests

server_url = "http://127.0.0.1:5000/send_message"

while True:
    user_input = input("$ ")
    try:
        response = requests.post(server_url, json={"message": user_input})
        if response.status_code == 200:
            print(response.json()['response'])
        else:
            print("Error from server:", response.status_code)
    except requests.exceptions.RequestException as e:
        print("Request failed:", e)