import random
import numpy as np
import sympy as sp
from math import gcd as bltin_gcd

class helper:
    def bezout(self, a, b):  # extended Euclid algorithm
        x, xx, y, yy = 1, 0, 0, 1
        while b:
            q = a // b
            a, b = b, a % b
            x, xx = xx, x - xx * q
            y, yy = yy, y - yy * q
        return (x, y, a)

    def euler(self, p, q):  # Euler function
        return (p - 1) * (q - 1)

    def primitive_root(self, modulo):  # and this is my realization of the primitive root
        for each in range(1, modulo):
            candidate_prim_roots = []
            for i in range(1, modulo):
                modulus = pow(each, i, modulo)
                if candidate_prim_roots.count(modulus) != 0:
                    break
                candidate_prim_roots.append(modulus)
                if i == modulo - 1:
                    return each


class ElGamal:
    def __init__(self):
        self.generateData()
    def generateData(self):
        self.p = int(random.random() * 10 ** 16 // 10**12)
        while not sp.isprime(self.p):
            self.p = int(random.random() * 10 ** 16 // 10 ** 12)
        self.g = helper().primitive_root(self.p)
        self.x = random.randint(2, self.p - 1)
        self.y = int(pow(self.g, self.x, self.p))
        print('p: ' + str(self.p) + ' g ' + str(self.g) + ' x: ' + str(self.x) + ' y: ' + str(self.y))
        return self.p, self.g, self.x, self.y

    def encrypt(self, text):
        encrypted = []
        for c in text:
            k = random.randint(1, self.p - 1)
            x, y, a = helper().bezout(k, self.p - 1)
            if x < 0:
                x += self.p
            gcd = a == 1
            while not gcd:
                k = random.randint(1, self.p - 1)
                x, y, a = helper().bezout(k, self.p - 1)
                if x < 0:
                    x += self.p
                gcd = a == 1

            encrypted.append(int(pow(self.g, k, self.p)))
            encrypted.append(ord(c) * pow(self.y, k, self.p) % self.p)
        print('enc ' + str(encrypted))
        return encrypted

    def decrypt(self, text):
        decrypted = []
        char = ''
        c = 0
        while c < len(text):
            char = pow(text[c], self.x, self.p)
            char, y, x = helper().bezout(char, self.p)
            c += 1
            char = int(text[c] * char % self.p)
            decrypted.append(chr(char))
            c += 1
        print('decr ' + str(decrypted))
        return decrypted


class RSA:
    p = 1
    q = 1
    n = 1
    phin = 1
    e = 1
    d = 1
    def __init__(self):
        self.generateData()
    def generateData(self):
        self.p = int(random.random() * 10 ** 16 // 10**8)
        while not sp.isprime(self.p):
            self.p = int(random.random() * 10 ** 16 // 10 ** 8)
        self.q = int(random.random() * 10 ** 16 // 10**8)
        while not sp.isprime(self.q):
            self.q = int(random.random() * 10 ** 16 // 10 ** 8)
        self.n = int(self.p * self.q)
        self.phin = int(helper().euler(self.p, self.q))
        self.e = random.randint(2, self.phin)
        x, y, a = helper().bezout(self.e, self.phin)
        if x < 0:
            x += self.phin
        gcd = a == 1
        while not sp.isprime(self.e) and not gcd:
            self.e = random.randint(2, self.phin)
            x, y, a = helper().bezout(self.e, self.phin)
            if x < 0:
                x += self.phin
            gcd = a == 1
        self.d = int(x)
        print('n: ' + str(self.n) + ' phin ' + str(self.phin) + ' p: ' + str(self.p) + ' q: ' + str(self.q) + ' e: ' + str(self.e) + ' d: ' + str(self.d))
        return self.p, self.q, self.e, self.d

    def encrypt(self, text):
        encrypted = []
        for c in text:
            encrypted.append(int(pow(ord(c), self.e, self.n)))
        print('enc ' + str(encrypted))
        return encrypted

    def decrypt(self, text):
        decrypted = []
        for c in text:
            decrypted.append(chr(pow(c, int(self.d), int(self.n))))
        print('decr ' + str(decrypted))
        return decrypted

if __name__ == '__main__':
    rsa = RSA()
    text = str(input())
    encrypted = rsa.encrypt(text)
    decrypted = rsa.decrypt(encrypted)
    el = ElGamal()
    enc = el.encrypt(text)
    decr = el.decrypt(enc)

# See PyCharm help at https://www.jetbrains.com/help/pycharm/
