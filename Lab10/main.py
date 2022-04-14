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

    #def digital_signature(self):



class RSA:
    p = []
    q = []
    n = []
    phin = []
    e = []
    d = []
    def __init__(self):
        print('Sender\'s data:')
        self.generate_data()
    def generate_data(self):
        p = int(random.random() * 10 ** 16 // 10**8)
        while not sp.isprime(p):
            p = int(random.random() * 10 ** 16 // 10 ** 8)
        self.p.append(p)
        q = int(random.random() * 10 ** 16 // 10**8)
        while not sp.isprime(q):
            q = int(random.random() * 10 ** 16 // 10 ** 8)
        self.q.append(q)
        n = (int(p * q))
        self.n.append(n)
        phin = int(helper().euler(p, q))
        self.phin.append(phin)
        e = random.randint(2, phin)
        x, y, a = helper().bezout(e, phin)
        if x < 0:
            x += phin
        gcd = a == 1
        while not sp.isprime(e) and not gcd:
            e = random.randint(2, phin)
            x, y, a = helper().bezout(e, phin)
            if x < 0:
                x += phin
            gcd = a == 1
        self.e.append(e)
        self.d.append(int(x))
        print('n: ' + str(n) + ' phin ' + str(phin) + ' p: ' + str(p) + ' q: ' + str(q) + ' e: ' + str(e) + ' d: ' + str(x))
        return p, q, e, x

    def encrypt(self, text, mode : bool):
        encrypted = []
        for c in text:
            if not mode:
                c = ord(c)
            encrypted.append(int(pow(c, self.e[mode], self.n[mode])))
        print('enc ' + str(encrypted))
        return encrypted

    def decrypt(self, text, mode : bool):
        decrypted = []
        for c in text:
            decrypted.append(pow(c, int(self.d[mode]), int(self.n[mode])))
        if not mode:
            for c in decrypted:
                print(chr(c), end='')
        print('\ndecr ' + str(decrypted))
        return decrypted

    def digital_signature(self, text):
        print('Recipient\'s data:')
        self.generate_data()
        verified_message = self.encrypt(text, False)
        encoded_verified_message = self.encrypt(verified_message, True)
        decoded_verified_message = self.decrypt(encoded_verified_message, True)
        applied_decoded_verified_message = self.decrypt(decoded_verified_message, False)
        print('Verified message: {}; Encoded verified message: {}; Decoded verified message: {}; '
              'Applied decoded verified message: {}'.format(
            verified_message, encoded_verified_message, decoded_verified_message, applied_decoded_verified_message))


if __name__ == '__main__':
    rsa = RSA()
    text = str(input())
    # encrypted = rsa.encrypt(text, False)
    # decrypted = rsa.decrypt(encrypted, False)
    el = ElGamal()
    enc = el.encrypt(text)
    decr = el.decrypt(enc)
    rsa.digital_signature(text)

# See PyCharm help at https://www.jetbrains.com/help/pycharm/
