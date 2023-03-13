import json
from socket import *

from clusterization import *
from model.Point import Point
from utilconfig import read_json_config

BUFFER_SIZE = 10 ** 10
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
    points = [Point(tuple([value["x"], value["y"], value["z"]]), number) for number, value in enumerate(data)]
    union_claster_worker = UnionClusterizationWorker(points)
    global_clusterization = union_claster_worker.make_clusterization(EXTERNAL_K_MAX, EXTERNAL_K_MIN)
    print("Этап глобальной кластеризации выполнен")
    ready_clusters = []
    for item in global_clusterization:
        union_claster_worker.worker.points = item.points
        ready_clusters.extend(union_claster_worker.make_clusterization(INTERNAL_K_MAX, INTERNAL_K_MIN))
    print("Этап кластеризации каждого кластера выполнен")
    spheres_list = []

    for cluster in ready_clusters:
        center = cluster.get_center()
        rad = cluster.get_internal_cluster_distance()
        spheres_list.append(
            {"center": {
                "x": center[0],
                "y": center[1],
                "z": center[2]
            }, "radious": rad / 1000}
        )
    msg = json.dumps({"spheres": spheres_list})
    print("Подготовка сфеерических областей выполнена")

    conn.send(msg.encode("utf-8"))
    print("Данные отправлены успешно!")
    conn.close()
