import math

from model.Point import Point


class DistanceWorker:
    @staticmethod
    def find_distance(point_one: Point, point_two: Point) -> float:
        if len(point_one.attributes) == len(point_two.attributes):
            return math.sqrt(sum(map(lambda a, b: math.pow(a - b, 2),
                                     point_one.attributes, point_two.attributes)))
        raise Exception("Points don\'t have similar attributes lengths!")
