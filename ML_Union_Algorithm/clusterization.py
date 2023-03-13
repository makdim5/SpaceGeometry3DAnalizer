import copy
import math
from functools import reduce
from model.Cluster import Cluster
from model.PointsWorker import PointsWorker

EXTERNAL_K_MAX = 20
EXTERNAL_K_MIN = 10
INTERNAL_K_MAX = 10
INTERNAL_K_MIN = 4


class UnionClusterizationWorker:

    def __init__(self, points):
        self.worker = PointsWorker()
        self.worker.points = points

    def make_clusterization(self, k_max, k_min, clusters=None):

        k_cluster_dict = {}
        distance_k_dict = {}
        clusters = clusters

        for k in range(k_max, k_min - 1, -1):
            print(f"Получаем {k}-кластаризацию")
            # получение и разметка кластеров
            clusters = [*self.worker.make_union_clustering(k, clusters)]
            print(f"Получаем знаки на {k}-кластаризацию")
            signs = [f"{i + 1} class" for i in range(len(clusters))]
            for cl, sign in zip(clusters, signs):
                cl.accept_class_sign(sign)

            # оценка кластеризации
            internal = reduce(lambda a, cl: a + cl.get_internal_cluster_distance(), clusters, 0) / len(clusters)
            external = Cluster([cl.get_center() for cl in clusters]).get_internal_cluster_distance()
            k_cluster_dict[k] = copy.deepcopy(clusters)
            distance_k_dict[math.fabs(internal - external)] = k

        return k_cluster_dict[distance_k_dict[min(distance_k_dict.keys())]]
