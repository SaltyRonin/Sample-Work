import numpy as np
import math

def rbmm(matrix_a, matrix_b, matrix_c):

    #If a dimension of any incoming matrix is 0 immeditately return, won't have any effect on calculation
    if(matrix_a.shape[0] == 0 or matrix_a.shape[1] == 0 or matrix_b.shape[0] == 0 or matrix_b.shape[1] == 0):
        return

    #Block dimension calculations
    m = math.floor(matrix_a.shape[0] / 2)
    p = math.floor(matrix_a.shape[1] / 2)
    n = math.floor(matrix_b.shape[0] / 2)
    j = math.floor(matrix_b.shape[1] / 2)
    c1 = math.floor(matrix_c.shape[0] / 2)
    c2 = math.floor(matrix_c.shape[1] / 2)

    #The blocks are cut  a00 = top left, a01 = top right, a10 = bottom left, a11 = bottom right

    #A
    matrix_a00 = matrix_a[0: m, 0:p]
    matrix_a01 = matrix_a[0: m, p: matrix_a.shape[1]]
    matrix_a10 = matrix_a[m: matrix_a.shape[0], 0: p]
    matrix_a11 = matrix_a[m: matrix_a.shape[0], p: matrix_a.shape[1]]

    #B
    matrix_b00 = matrix_b[0: n, 0:j]
    matrix_b01 = matrix_b[0: n, j: matrix_b.shape[1]]
    matrix_b10 = matrix_b[n: matrix_b.shape[0], 0: j]
    matrix_b11 = matrix_b[n: matrix_b.shape[0], j: matrix_b.shape[1]]

    #C
    matrix_c00 = matrix_c[0: c1, 0:c2]
    matrix_c01 = matrix_c[0: c1, c2: matrix_c.shape[1]]
    matrix_c10 = matrix_c[c1: matrix_c.shape[0], 0: c2]
    matrix_c11 = matrix_c[c1: matrix_c.shape[0], c2: matrix_c.shape[1]]


    #Base Case
    if(matrix_a.shape[0] == 1 and matrix_a.shape[1] == 1 and matrix_b.shape[0] == 1 and matrix_b.shape[1] == 1):
        #Once the 1x1 base case is met all the recursive calls will unravel executing this line
        matrix_c += np.dot(matrix_a, matrix_b)
    else:
        #For each rbmm call there are 4 recursive calls, one for each respective block, 4^n until 1x1 blocks are found
        #C00
            rbmm(matrix_a00, matrix_b00, matrix_c00)
            rbmm(matrix_a01,matrix_b10, matrix_c00)
        #C01
            rbmm(matrix_a00, matrix_b01, matrix_c01)
            rbmm(matrix_a01, matrix_b11, matrix_c01)
        #C10
            rbmm(matrix_a10, matrix_b00, matrix_c10)
            rbmm(matrix_a11, matrix_b10, matrix_c10)
        #C11
            rbmm(matrix_a10, matrix_b01, matrix_c11)
            rbmm(matrix_a11, matrix_b11, matrix_c11)


def driver(a,b,output):
    #Loads 1 matrix in per file, will have accurate dimensions
    matrix_a = np.loadtxt(a)
    matrix_b = np.loadtxt(b)

    #Checks to make sure that the matrices are multiplication compatible
    m = matrix_a.shape[1] 
    n = matrix_b.shape[0]

    if(m != n):
        print("Matrices are incompatible for multiplication.")
    else:
        #np.dot(matrix_a, matrix_b) is regular matrix multiplication to check answer
        matrix_z = np.dot(matrix_a, matrix_b)
        print(matrix_z)
        #Create a matrix with the right dimensions to hold a*b, fill it with zeroes

        matrix_c = np.zeros((matrix_a.shape[0], matrix_b.shape[1]))
        rbmm(matrix_a, matrix_b, matrix_c)
        np.savetxt(output, matrix_c)
        print(matrix_c)

#Files that are read from and written to
driver('G:\TestFile\matrix_c.txt','G:\TestFile\matrix_d.txt', "G:\TestFile\output.txt")