from typing import Tuple


class Point:
    def __init__(self, attributes: Tuple[float | int, ...],
                 number: int = 0):
        self.attributes = attributes
        self.number = number

    def __repr__(self):
        return f"{self.number}) Point({self.attributes}"
