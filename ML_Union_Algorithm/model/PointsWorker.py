import math
from typing import List

from model.Cluster import Cluster
from model.Point import Point
from util.DistanceWorker import DistanceWorker
from util.MatrixHelper import MatrixHelper


class PointsWorker:
    def __init__(self):
        self.points = []
        self.distances = {}

    def make_union_clustering(self, k, current_classes: List[Cluster] = None):
        classes = current_classes if current_classes else [Cluster([point]) for point in self.points]
        start = len(classes)
        for _ in range(start, k, -1):
            distances = MatrixHelper.create_square_matrix_per_rule(DistanceWorker.find_distance, classes)
            i, j = MatrixHelper.find_min_element_coords_in_matrix(distances)

            new_class = classes[i] + classes[j]
            classes = [classes[h] for h in range(len(classes)) if h not in {i, j}]
            classes.append(new_class)

        return classes

    def define_class_for_point(self, point, k):
        min_distances = self.get_k_min_distances(point, k)

        res = None
        if len(min_distances) == len({p.class_definition for p in min_distances.keys()}):
            min_value = min([val for val in min_distances.values()])

            for key, value in min_distances.items():
                if value == min_value:
                    res = key.class_definition
                    break
        else:
            cl_dict = {}

            for key, value in min_distances.items():
                if key not in cl_dict.keys():
                    cl_dict[key.class_definition] = 1
                else:
                    cl_dict[key.class_definition] += 1
            max_val = max([val for val in cl_dict.values()])

            for key, value in cl_dict.items():
                if value == max_val:
                    res = key
                    break

        return res

    def get_k_min_distances(self, new_point: Point, counter: int):
        self.distances = {point: DistanceWorker.find_distance(point, new_point)
                          for point in self.points}

        new_sorted_dict = [(k, v) for k, v in self.distances.items()]

        length = len(new_sorted_dict)
        for i in range(length):
            # Внутренний цикл, N-i-1 проходов
            for j in range(0, length - i - 1):
                # Меняем элементы местами
                array = new_sorted_dict
                if array[j][1] > array[j + 1][1]:
                    temp = array[j]
                    array[j] = array[j + 1]
                    array[j + 1] = temp

        new_sorted_dict = new_sorted_dict[:counter]

        sd = {k[0]: k[1] for k in new_sorted_dict}

        return sd

    def get_coords_per_type(self, cl_type):
        return tuple(map(lambda attr_num: [point.attributes[attr_num]
                                           for point in self.points
                                           if point.class_definition == cl_type],
                         range(len(self.points[0].attributes))))
