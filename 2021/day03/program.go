package main

import (
	"bytes"
	"fmt"
	"math"
	"os"
	"time"
)

func main() {
	if len(os.Args) < 2 {
		panic("must provide input file name as first command line argument")
	}
	fileName := os.Args[1]

	var start time.Time = time.Now()
	lines := ReadFileLines(fileName)
	bits := ParseInput(lines)
	fmt.Printf("File read: %s\n", time.Since(start))

	start = time.Now()
	part1 := Part1(bits)
	fmt.Printf("Part 1: %d (%s)\n", part1, time.Since(start))

	start = time.Now()
	part2 := Part2(bits)
	fmt.Printf("Part 2: %d (%s)\n", part2, time.Since(start))
}

func Part1(bits [][]int64) int64 {
	var gammaValues []int64
	var epsilonValues []int64

	lineCount := len(bits)
	lineLength := len(bits[0])

	for column := range lineLength {
		commonBytes := map[int64]int{
			int64(0): 0,
			int64(1): 0,
		}

		for row := range lineCount {
			value := bits[row][column]
			commonBytes[value] += 1
		}

		var minValue int64
		var maxValue int64
		minCount := math.MaxInt64
		maxCount := math.MinInt64

		for value, count := range commonBytes {
			if count > maxCount {
				maxCount = count
				maxValue = value
			}
			if count < minCount {
				minCount = count
				minValue = value
			}
		}

		gammaValues = append(gammaValues, maxValue)
		epsilonValues = append(epsilonValues, minValue)
	}

	gammaRate := int64(0)
	for index, value := range gammaValues {
		gammaRate += value << (lineLength - 1 - index)
	}
	epsilonRate := int64(0)
	for index, value := range epsilonValues {
		epsilonRate += value << (lineLength - 1 - index)
	}
	return gammaRate * epsilonRate
}

func Part2(bits [][]int64) int64 {
	lineLength := len(bits[0])

	var oxygenGeneratorPossibleValues [][]int64
	var co2ScrubberPossibleValues [][]int64

	for _, value := range bits {
		oxygenGeneratorPossibleValues = append(oxygenGeneratorPossibleValues, value)
		co2ScrubberPossibleValues = append(co2ScrubberPossibleValues, value)
	}

	var commonBytes map[int64]int

	for column := range lineLength {
		// Find value for oxygen generator
		if len(oxygenGeneratorPossibleValues) > 1 {
			commonBytes = map[int64]int{
				int64(0): 0,
				int64(1): 0,
			}

			for row := range len(oxygenGeneratorPossibleValues) {
				value := oxygenGeneratorPossibleValues[row][column]
				commonBytes[value] += 1
			}

			var maxValue int64
			maxCount := math.MinInt64

			for value, count := range commonBytes {
				if count > maxCount {
					maxCount = count
					maxValue = value
				} else if count == maxCount {
					// If the count is the same we need to default to 1
					maxValue = 1
				}
			}

			var filteredPossibleValues [][]int64
			for row := range len(oxygenGeneratorPossibleValues) {
				if oxygenGeneratorPossibleValues[row][column] == maxValue {
					filteredPossibleValues = append(filteredPossibleValues, oxygenGeneratorPossibleValues[row])
				}
			}
			oxygenGeneratorPossibleValues = filteredPossibleValues
		}

		// Find value for CO2 scrubber
		if len(co2ScrubberPossibleValues) > 1 {
			commonBytes = map[int64]int{
				int64(0): 0,
				int64(1): 0,
			}

			for row := range len(co2ScrubberPossibleValues) {
				value := co2ScrubberPossibleValues[row][column]
				commonBytes[value] += 1
			}

			var minValue int64
			minCount := math.MaxInt64

			for value, count := range commonBytes {
				if count < minCount {
					minCount = count
					minValue = value
				} else if count == minCount {
					// If the count is the same we need to default to 0
					minValue = 0
				}
			}

			var filteredPossibleValues [][]int64
			for row := range len(co2ScrubberPossibleValues) {
				if co2ScrubberPossibleValues[row][column] == minValue {
					filteredPossibleValues = append(filteredPossibleValues, co2ScrubberPossibleValues[row])
				}
			}
			co2ScrubberPossibleValues = filteredPossibleValues
		}
	}

	oxygenGeneratorRating := int64(0)
	for index, value := range oxygenGeneratorPossibleValues[0] {
		oxygenGeneratorRating += value << (lineLength - 1 - index)
	}

	co2ScrubberRating := int64(0)
	for index, value := range co2ScrubberPossibleValues[0] {
		co2ScrubberRating += value << (lineLength - 1 - index)
	}

	return oxygenGeneratorRating * co2ScrubberRating
}

func ParseInput(lines [][]byte) [][]int64 {
	var numbers [][]int64

	for _, line := range lines {
		var bits []int64
		for _, bit := range line {
			bits = append(bits, int64(bit-byte('0')))
		}
		numbers = append(numbers, bits)
	}

	return numbers
}

func ReadFileLines(fileName string) [][]byte {
	data, err := os.ReadFile(fileName)
	if err != nil {
		panic(err)
	}
	lines := bytes.Split(data, []byte("\n"))

	return lines
}
