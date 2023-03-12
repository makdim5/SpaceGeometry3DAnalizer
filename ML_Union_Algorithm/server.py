import json
from socket import *

from utilconfig import read_json_config

BUFFER_SIZE = 10**10
config = read_json_config(filename="env.json")

host = config["host"]
port = config["port"]
address = (host, port)

tcp_socket = socket(AF_INET, SOCK_STREAM)
tcp_socket.bind(address)
tcp_socket.listen(1)
print('UnionService для кластеризации запущен...')
while True:
    conn, address = tcp_socket.accept()
    data = conn.recv(BUFFER_SIZE).decode("utf-8")
    data = json.loads(data)
    spheres = {
        "spheres": [
            {"center": {
                "x": -5,
                "y": 2,
                "z": -2
            }, "radious": 10.0/1000}
        ]
    }
    msg = json.dumps(spheres)

    conn.send(msg.encode("utf-8"))
    print("Данные отправлены успешно!")
    conn.close()
