# coding: utf-8
import matplotlib.pyplot as plt
import numpy as np

data = np.genfromtxt("output.txt", skip_header=1, delimiter='\t')

time = data[:, 0]
energy = data[:, 1]
angular_momentum = data[:, 2]
pos_x = data[:, 3]
pos_y = data[:, 4]

# Create figure and subplots
fig, (ax1, ax2, ax3) = plt.subplots(1, 3, figsize=(15, 5))

# Plot orbit
ax1.plot(pos_x, pos_y)
ax1.set_xlabel("x position")
ax1.set_ylabel("y position")
ax1.set_title("Orbit")
ax1.set_aspect('equal', 'datalim')
ax1.annotate('☉', xy=(0, 0), xytext=(0, 0), ha='center', va='center', fontsize=24)

# Plot energy
ax2.plot(time, energy)
ax2.set_xlabel("time")
ax2.set_ylabel("energy (joules)")
ax2.set_title("Energy vs. Time")

# Plot angular momentum
ax3.plot(time, angular_momentum)
ax3.set_xlabel("time")
ax3.set_ylabel("angular momentum")
ax3.set_title("Angular Momentum vs. Time")


plt.show()
