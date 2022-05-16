import random
import numpy as np
import sympy as sp
import MD5 as hash
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
        self.generate_data()

    def generate_data(self):
        self.p = random.randint(4096, 8191)
        while not sp.isprime(self.p):
            self.p = random.randint(4096, 8191)
        self.g = helper().primitive_root(self.p)
        x, y, a = helper().bezout(self.g, self.p)
        if x < 0:
            x += self.p
        gcd = a == 1
        while not gcd:
            self.g = helper().primitive_root(self.p)
            x, y, a = helper().bezout(self.g, self.p)
            if x < 0:
                x += self.p
            gcd = a == 1

        self.x = random.randint(2, self.p - 2)
        self.y = pow(self.g, self.x, self.p)
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

    def digital_signature(self, text):
        self.b = 0
        self.a = 0
        while self.b == 0:
            k = random.randint(2, self.p - 2)
            x, y, a = helper().bezout(k, self.p - 1)
            if x < 0:
                x += self.p
            gcd = a == 1
            while not gcd:
                k = random.randint(1, self.p - 2)
                x, y, a = helper().bezout(k, self.p - 1)
                if x < 0:
                    x += self.p
                gcd = a == 1
            hash_func = hash.MD5()
            H = int(hash_func.hash(text)[0:5], 16)
            self.a = pow(self.g, k, self.p)
            self.b = (H - self.x * self.a) * x % (self.p - 1)
        print('Digital signature: ({}, {})'.format(self.a, self.b))

    def check_digital_signature(self, text):
        hash_func = hash.MD5()
        H = int(hash_func.hash(text)[0:5], 16)
        modulus = pow(self.g, H, self.p)
        if modulus == pow(self.y, self.a, self.p) * pow(self.a, self.b, self.p) % self.p:
            print("Success")
        else:
            print('Failed')


class Shnorr:

    def __init__(self):
        self.generate_data()

    def generate_data(self):
        self.p = int(random.random() * 10 ** 16 // 10**8)
        while not sp.isprime(self.p):
            self.p = int(random.random() * 10 ** 16 // 10 ** 8)
        self.q = (self.p - 1) // 2
        for c in range((self.p - 1) // 2, 2):
            if sp.isprime(c) and (self.p - 1) / c == 0:
                self.q = c
                break
        modulus = 0
        self.g = 0
        while modulus != 1:
            self.g = int(random.random() * 10 ** 16 // 10 ** 8)
            modulus = pow(self.g, self.q, self.p)
        self.w = random.randint(1, self.q)
        self.y = pow(self.g, self.q - self.w, self.p)
        print('Public key (p, g, q, y): ({0}, {1}, {2}, {3})'.format(self.p, self.g, self.q, self.y));
        print('Private key w: ' + str(self.w))

    def digital_signature(self):
        self.b = 0
        self.a = 0
        k = random.randint(2, self.q)
        # x, y, a = helper().bezout(k, self.p - 1)
        # if x < 0:
        #     x += self.p
        # gcd = a == 1
        # while not gcd:
        #     k = random.randint(1, self.q)
        #     x, y, a = helper().bezout(k, self.p - 1)
        #     if x < 0:
        #         x += self.p
        #     gcd = a == 1
        print('k:' + str(k))
        self.a = pow(self.g, k, self.p)
        self.e = random.randint(0, 2 ** 16 - 1)
        self.s = (k + self.w * self.e) % self.q
        if self.s == pow(self.g, self.s, self.p) * pow(self.y, self.e, self.p) % self.p:
            print('Success')
        else:
            print('Failure')
            # self.b = (H - self.x * self.a) * x % (self.p - 1)
        #print('Digital signature: ({}, {})'.format(self.a, self.b))


class RSA:
    p = []
    q = []
    n = []
    phin = []
    e = []
    d = []
    def __init__(self):
        print('\nSender\'s data:')
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
        print('\nRecipient\'s data:')
        self.generate_data()
        verified_message = self.encrypt(text, False)
        encoded_verified_message = self.encrypt(verified_message, True)
        decoded_verified_message = self.decrypt(encoded_verified_message, True)
        applied_decoded_verified_message = self.decrypt(decoded_verified_message, False)
        print('Verified message: {}; Encoded verified message: {}; Decoded verified message: {}; '
              'Applied decoded verified message: {}'.format(
            verified_message, encoded_verified_message, decoded_verified_message, applied_decoded_verified_message))


if __name__ == '__main__':
    text = str(input())

    el = ElGamal()
    enc = el.encrypt(text)
    decr = el.decrypt(enc)
    el.digital_signature(text)
    el.check_digital_signature(text)

    shnorr = Shnorr()
    shnorr.digital_signature()

    rsa = RSA()
    encrypted = rsa.encrypt(text, False)
    decrypted = rsa.decrypt(encrypted, False)
    rsa.digital_signature(text)


# See PyCharm help at https://www.jetbrains.com/help/pycharm/
