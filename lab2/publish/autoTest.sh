#!/bin/bash
modeFirstTest="mealy"
testFirstPathInput="2. Минимизация/input-mealy.csv"
testFirstPathOutput="2. Минимизация/poutpute-mealy.csv"
toCheckFirstResult="2. Минимизация/outpute-mealy.csv"

modeSecondTest="moore"
testSecondPathInput="2. Минимизация/input-moore.csv"
testSecondPathOutput="2. Минимизация/poutput-moore.csv"
toCheckSecondResult="2. Минимизация/output-moore.csv"

./lw2 "$modeFirstTest" "$testFirstPathInput" "$testFirstPathOutput"
./lw2 "$modeSecondTest" "$testSecondPathInput" "$testSecondPathOutput"

