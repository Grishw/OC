#!/bin/bash
modeFirstTest="mealy-to-moore"
testFirstPathInput="1. Конвертация Мили-Мура/input-mealy.csv"
testFirstPathOutput="1. Конвертация Мили-Мура/poutpute-moore.csv"
toCheckFirstResult="1. Конвертация Мили-Мура/outpute-moore.csv"

modeSecondTest="moore-to-mealy"
testSecondPathInput="1. Конвертация Мили-Мура/input-moore.csv"
testSecondPathOutput="1. Конвертация Мили-Мура/poutput-mealy.csv"
toCheckSecondResult="1. Конвертация Мили-Мура/output-mealy.csv"

./lw1 "$modeFirstTest" "$testFirstPathInput" "$testFirstPathOutput"
./lw1 "$modeSecondTest" "$testSecondPathInput" "$testSecondPathOutput"

