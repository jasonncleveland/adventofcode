package day06

import (
	"bytes"
	"fmt"
	"os"
	"time"
)

func Run(fileName string) {
	var start time.Time = time.Now()
	lines := ReadFileLines(fileName)
	fmt.Printf("File read: %s\n", time.Since(start))

	start = time.Now()
	part1 := Part1(lines)
	fmt.Printf("Part 1: %d (%s)\n", part1, time.Since(start))

	start = time.Now()
	part2 := Part2(lines)
	fmt.Printf("Part 2: %d (%s)\n", part2, time.Since(start))
}

func Part1(lines [][]byte) int64 {
	numbers := ParseInput(lines)[0]
	lanternFishMap := make(map[int64]int64)

	for _, number := range numbers {
		lanternFishMap[number] += 1
	}

	for range 80 {
		mapCopy := make(map[int64]int64)
		for key, value := range lanternFishMap {
			if key == 0 {
				// Create a new fish with a lifespan of 8 and reset to 6
				mapCopy[8] += value
				mapCopy[6] += value
			} else {
				// Decrease lifespan
				mapCopy[key-1] += value
			}
		}
		lanternFishMap = mapCopy
	}

	total := int64(0)
	for _, value := range lanternFishMap {
		total += value
	}
	return total
}

func Part2(lines [][]byte) int64 {
	numbers := ParseInput(lines)[0]
	lanternFishMap := make(map[int64]int64)

	for _, number := range numbers {
		lanternFishMap[number] += 1
	}

	for range 256 {
		mapCopy := make(map[int64]int64)
		for key, value := range lanternFishMap {
			if key == 0 {
				// Create a new fish with a lifespan of 8 and reset to 6
				mapCopy[8] += value
				mapCopy[6] += value
			} else {
				// Decrease lifespan
				mapCopy[key-1] += value
			}
		}
		lanternFishMap = mapCopy
	}

	total := int64(0)
	for _, value := range lanternFishMap {
		total += value
	}
	return total
}

func ParseInput(lines [][]byte) [][]int64 {
	var data [][]int64

	for _, line := range lines {
		var numbers []int64
		for _, number := range bytes.Split(line, []byte(",")) {
			numbers = append(numbers, ParseNumber(number))
		}
		data = append(data, numbers)
	}

	return data
}

func ReadFileLines(fileName string) [][]byte {
	data, err := os.ReadFile(fileName)
	if err != nil {
		panic(err)
	}
	lines := bytes.Split(data, []byte("\n"))

	return lines
}

func ParseNumber(bytes []byte) int64 {
	magnitude := int64(1)
	number := int64(0)
	for index := range bytes {
		number += int64(bytes[len(bytes)-1-index]-byte('0')) * magnitude
		magnitude *= 10
	}
	return number
}
