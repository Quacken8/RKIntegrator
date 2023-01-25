# coding: utf-8
import numpy as np
import matplotlib.pyplot as plt

legends = []

EulerStepsizes = []
EulerPositions = []

RK2Stepsizes = []
RK2Positions = []

RK4Stepsizes = []
RK4Positions = []

ComboPositions = [EulerPositions, RK2Positions, RK4Positions]
ComboStepsizes = [EulerStepsizes, RK2Stepsizes, RK4Stepsizes]

with open('stepsizeTestOutput.txt', 'r') as f:
    # Skip the header line
    next(f)
    
    lines = f.readlines()
    
    for i in range(3):
            
        for j in range(10):
            currentLine = lines[i*10+j]
            if j == 0: legends.append(currentLine)
            else: 
                ComboStepsizes[i].append(currentLine.split()[0])
                ComboPositions[i].append(currentLine.split()[2])

plt.scatter(np.array(EulerStepsizes, dtype = np.double), np.array(EulerPositions, dtype = np.double), label = legends[0])
plt.scatter(np.array(RK2Stepsizes, dtype = np.double), np.array(RK2Positions, dtype = np.double), label = legends[1])
plt.scatter(np.array(RK4Stepsizes, dtype = np.double), np.array(RK4Positions, dtype = np.double), label = legends[2])

plt.legend()

plt.xscale('log')
plt.yscale('log')
plt.xlabel('stepSize')
plt.ylabel('position')

plt.show()
