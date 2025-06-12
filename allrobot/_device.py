
# 自動生成されるのでここにプログラムを書かないで!!
import sys
class Motor:
    def __init__(self,name):
        self.name=name
        self._power=0
    @property
    def power(self):
        return self._power
    @power.setter
    def power(self, value):
        if self._power != value:
            print(f"set: {self.name}.power = {value}")
            sys.stdout.flush() 
            self._power = value

class Servo:
    def __init__(self, name):
        self.name = name
        self._angle = 0
    @property
    def angle(self):
        return self._angle
    @angle.setter
    def angle(self, value):
        if self._angle != value:
            print(f"set: {self.name}.angle = {value}")
            sys.stdout.flush() 
            self._angle = value
            
class DistanceSenser:
    def __init__(self, name):
        pass
    @property
    def angle(self):
        return 0

class Light:
    def __init__(self, name):
        self.name = name
        self._intensity = 0
    @property
    def intensity(self):
        return self._intensity
    @intensity.setter
    def intensity(self, value):
        if self._intensity != value:
            print(f"set: {self.name}.intensity = {value}")
            sys.stdout.flush() 
            self._intensity = value
class Camera:
    def __init__(self, name):
        self.name = name
class Orbitcamera:
    def __init__(self, name):
        self.name = name
1 = Distancesensor("1")
L = Distancesensor("L")
R = Distancesensor("R")
C = Distancesensor("C")
B = Motor("B")
A = Motor("A")
camera = Orbitcamera("camera")
camera = Camera("camera")
C = Motor("C")
H = Hinge("H")
C = Servo("C")
B = Hinge("B")
A = Hinge("A")
D1 = Servo("D1")
C2 = Servo("C2")
B2 = Servo("B2")
D2 = Servo("D2")
C1 = Servo("C1")
B1 = Servo("B1")
A2 = Servo("A2")
A1 = Servo("A1")
B = Servo("B")
A = Servo("A")
D = Servo("D")
L = Light("L")
