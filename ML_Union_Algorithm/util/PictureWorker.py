from typing import Tuple
import numpy
import cv2


class PictureWorker:
    CHAR_COLOR = numpy.array([0, 0, 0])
    GREEN_COLOR = numpy.array([76, 177, 34])
    RED_COLOR = numpy.array([36, 28, 237])
    ZOND_IMG = cv2.imread('util\\zonds.png')
    BLENDED_IMG_LOCATION = "data\\blended.jpg"

    @classmethod
    def get_zond_coords(cls, img_path: str) -> Tuple[int, int]:

        img = cv2.imread(img_path)

        dst = cv2.addWeighted(img, 0.5, cls.ZOND_IMG, 0.7, 0)
        pict_size = len(img)
        one_coords = [[0 for j in range(pict_size)] for i in range(pict_size)]
        two_coords = [[0 for j in range(pict_size)] for i in range(pict_size)]

        for i in range(pict_size):
            for j in range(pict_size):
                if numpy.array_equal(img[i][j], cls.CHAR_COLOR) and numpy.array_equal(cls.ZOND_IMG[i][j],
                                                                                      cls.GREEN_COLOR):
                    one_coords[i][j] = 1
                elif numpy.array_equal(img[i][j], cls.CHAR_COLOR) and numpy.array_equal(cls.ZOND_IMG[i][j],
                                                                                        cls.RED_COLOR):
                    two_coords[i][j] = 1
        one_coords = PictureWorker.analize_matrix_coords(one_coords)
        two_coords = PictureWorker.analize_matrix_coords(two_coords)
        return one_coords, two_coords

    @staticmethod
    def analize_matrix_coords(matrix_coords):
        coords_counter = 0

        for i in range(len(matrix_coords)):
            for j in range(len(matrix_coords)):
                if matrix_coords[i][j] == 1:
                    coords_counter += 1
                    matrix_coords = PictureWorker.clear_matrix(matrix_coords, i, j)

        return coords_counter

    @staticmethod
    def clear_matrix(matrix, i, j):
        trend_i, trend_j, increment_i, increment_j = 0, 0, 0, 0

        if i - 1 > 0 and j - 1 > 0:
            if matrix[i - 1][j - 1] == 1:
                increment_i, increment_j = -1, -1
                trend_i, trend_j = i - 1, j - 1

        if j - 1 > 0:
            if matrix[i][j - 1] == 1:
                increment_j = -1
                trend_i, trend_j = i, j - 1

        if i + 1 < len(matrix) and j - 1 > 0:
            if matrix[i + 1][j - 1] == 1:
                increment_i, increment_j = 1, -1
                trend_i, trend_j = i + 1, j - 1

        if i - 1 > 0:
            if matrix[i - 1][j] == 1:
                increment_i = -1
                trend_i, trend_j = i - 1, j

        if i + 1 < len(matrix):
            if matrix[i + 1][j] == 1:
                increment_i = 1
                trend_i, trend_j = i + 1, j

        if i - 1 > 0 and j + 1 < len(matrix):
            if matrix[i - 1][j + 1] == 1:
                increment_i, increment_j = -1, 1
                trend_i, trend_j = i - 1, j + 1

        if j + 1 < len(matrix):
            if matrix[i][j + 1] == 1:
                increment_j = 1
                trend_i, trend_j = i, j + 1

        if i + 1 < len(matrix) and j + 1 < len(matrix):
            if matrix[i + 1][j + 1] == 1:
                increment_i, increment_j = 1, 1
                trend_i, trend_j = i + 1, j + 1

        while 0 <= trend_i < len(matrix) and 0 <= trend_i < len(matrix) and matrix[trend_i][trend_j]:
            matrix[trend_i][trend_j] = 0
            trend_i += increment_i
            trend_j += increment_j

        return matrix

    @classmethod
    def write_zond_overlay(cls, img_path, overlay_img_path=BLENDED_IMG_LOCATION):
        img = cv2.imread(img_path)
        dst = cv2.addWeighted(img, 0.5, cls.ZOND_IMG, 0.7, 0)
        cv2.imwrite(overlay_img_path, dst)