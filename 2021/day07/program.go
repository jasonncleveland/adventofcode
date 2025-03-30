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

	var max int64 = math.MinInt64
	var min int64 = math.MaxInt64

	for _, number := range numbers {
		if number > max {
			max = number
		}
		if number < min {
			min = number
		}
	}

	var minFuel int64 = math.MaxInt64
	for step := range max {
		fuel := int64(0)
		for _, crab := range numbers {
			fuel += int64(math.Abs(float64(crab - step)))
		}
		if fuel < minFuel {
			minFuel = fuel
		}
	}

	return minFuel
}

func Part2(lines [][]byte) int64 {
	return -1
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
