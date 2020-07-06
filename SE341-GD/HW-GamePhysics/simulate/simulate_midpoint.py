#!/usr/bin/env python3

from math import sin, cos, pi, atan2
from time import sleep
from random import random

theta_rad = 20
theta = theta_rad * pi / 180
omega = 0.0

world_g = 9.80665
length = 1

time_elapse = 0.02
current_time = 0

csv_lines = []

counter = 0

try:
    while True:
        current_time += time_elapse

        halfTime = time_elapse / 2.0
        midTheta = theta + omega * halfTime
        midOmega = omega - world_g / length * sin(theta) * halfTime


        theta += midOmega * time_elapse
        omega -= world_g / length * sin(midTheta) * time_elapse

        sleep(time_elapse)

        print("time: %.3f theta: %.2f omega: %.2f" %
              (current_time, theta * 180 / pi, omega), end='\r')

        csv_lines.append("%f, %f, %f\n" %
                         (current_time, theta * 180 / pi, omega))

except KeyboardInterrupt:
    print("\ninterrupted")

filename = input("\ncsv file name? >>> ")

with open(filename, 'w') as f:
    f.writelines(csv_lines)

print("ok")
