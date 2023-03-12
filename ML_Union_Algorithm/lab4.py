import math
from functools import reduce
from matplotlib import pyplot as plt
from model.Cluster import Cluster
from model.PointsWorker import PointsWorker
from util.ColorGenerator import ColorGenerator
from util.ExcelWorker import ExcelWorker

# чтение файла и получение точек
EXCEL_FILE_LOCATION = 'data\\nodes.xls'
df = ExcelWorker.read(EXCEL_FILE_LOCATION)
print(df)
worker = PointsWorker()
worker.points = ExcelWorker.define_points_in_df(df, tuple(["x", "y", "z"]))

K_MAX = 20
K_MIN = 10

diffs = []
clusters = None

for k in range(K_MAX, K_MIN - 1, -1):
    print(f"Получаем {k}-кластаризацию")
    # получение и разметка кластеров
    clusters = [*worker.make_union_clustering(k, clusters)]
    print(f"Получаем знаки на {k}-кластаризацию")
    signs = [f"{i + 1} class" for i in range(len(clusters))]
    for cl, sign in zip(clusters, signs):
        cl.accept_class_sign(sign)

    # вывод на картинке
    colors = [ColorGenerator.hex_generate() for _ in signs]

    fig = plt.figure()
    ax = fig.add_subplot(111, projection='3d')
    for color, sign in zip(colors, signs):
        ax.scatter(*worker.get_coords_per_type(sign), color=color)

    plt.xlabel(f'clustering {k} parts')
    plt.show()

    # оценка кластеризации
    internal = reduce(lambda a, cl: a + cl.get_internal_cluster_distance(), clusters, 0) / len(clusters)
    external = Cluster([cl.get_center() for cl in clusters]).get_internal_cluster_distance()
    diffs.append(math.fabs(internal - external))

p, = plt.plot([i for i in range(K_MIN, K_MAX + 1)], diffs[::-1])
p.set_marker("o")
plt.xlabel('Lab 4 Makanov GENERAL')
plt.show()
