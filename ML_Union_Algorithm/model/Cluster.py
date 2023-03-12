from functools import reduce
from typing import List

from model.Point import Point
from util.DistanceWorker import DistanceWorker


class Cluster:
    def __init__(self, points: List[Point]):
        self.points: List[Point] = points

    def get_center(self):
        attrs = tuple(map(lambda i: reduce(lambda a, point: a + point.attributes[i], self.points, 0) /
                                    len(self.points),
                          range(len(self.points[0].attributes))))
        return Point(attrs)

    def get_internal_cluster_distance(self) -> float:
        current_center = self.get_center()
        return reduce(lambda a, point: a + DistanceWorker.find_distance(current_center, point), self.points, 0) / len(
            self.points)

    def accept_class_sign(self, sign: str):
        for point in self.points:
            point.class_definition = sign

    def __add__(self, other):
        return Cluster(self.points + other.points)

    def __repr__(self):
        return f"Cluster - {self.points}"
