# This is a sample Python script.
import math
import numpy as np
from ctypes import c_int32
# Press Shift+F10 to execute it or replace it with your code.
# Press Double Shift to search everywhere for classes, files, tool windows, actions, and settings.
class MD5:
    length = 512
    tempText = ''
    k = []
    s = []
    def fill_k(self):
        for c in range(0, 64):
            self.k.append(math.floor(2**32 * math.fabs(math.sin(c + 1))))
        self.a0 = 0x67452301
        self.b0 = 0xefcdab89
        self.c0 = 0x98badcfe
        self.d0 = 0x10325476

        self.s[0:16] = [7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22, 7, 12, 17, 22]
        self.s[16:32] = [5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20, 5, 9, 14, 20]
        self.s[32:48] = [4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23, 4, 11, 16, 23]
        self.s[48:64] = [6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21, 6, 10, 15, 21]

    def first_step(self, text):
        self.text = text
        for c in text:
            self.tempText += str(bin(ord(c))).removeprefix('0b')
        self.tempText += '1'
        while len(self.tempText) % self.length != 448:
            self.tempText += '0'

        print(self.tempText)

    def second_step(self):
        bin_length = bin(len(self.text)*8).removeprefix('0b')
        if 64 - len(bin_length) > 0:
            bin_length = (64 - len(bin_length)) * '0' + bin_length
        self.tempText += bin_length[-33:-1] + bin_length[-64:-32]
        print(str(len(self.tempText)) + ' ' + bin_length)
        print(self.tempText)

    def third_step(self):
        chunks = [self.tempText[x:x + 512] for x in range(0, len(self.tempText), 512)]
        print(chunks)
        for c in chunks:
            word32 = [c[x: x + 32] for x in range(0, len(c), 32)]
            print(word32)
            A = self.a0
            B = self.b0
            C = self.c0
            D = self.d0
            for i in range(0, 64):
                if 0 <= i <= 15:
                    F = (B and C) or ((not B) and D)
                    g = i
                elif 16 <= i <= 31:
                    F = (D and B) or ((not D) and C)
                    g = (5 * i + 1) % 16
                elif 32 <= i <= 47:
                    F = B ^ C ^ D
                    g = (3 * i + 5) % 16
                elif 48 <= i <= 63:
                    F = C ^ (B or (not D))
                    g = (7*i) % 16
                F = int('0b' + bin(F + A + self.k[i] + int(word32[g], 2)).removeprefix('0b')[-33:-1] ,2)
                A = D
                D = C
                C = B
                B = int('0b' + bin(B + int(F) << int(self.s[i])).removeprefix('0b')[-33:-1], 2)
            self.a0 += A
            self.b0 += B
            self.c0 += C
            self.d0 += D
        np_digest = np.array([hex(self.a0), hex(self.b0), hex(self.c0), hex(self.d0)])
        print(np_digest)

def print_hi(name):
    # Use a breakpoint in the code line below to debug your script.
    print(f'Hi, {name}')  # Press Ctrl+F8 to toggle the breakpoint.


# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    md = MD5()
    md.fill_k()
    text = str(input())
    md.first_step(text)
    md.second_step()
    md.third_step()

# See PyCharm help at https://www.jetbrains.com/help/pycharm/
