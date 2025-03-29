package main

import (
	"bytes"
	"fmt"
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
	numbers := ParseInput(lines)

	part1 := Part1(numbers)
	fmt.Printf("Part 1: %d (%s)\n", part1, time.Since(start))
}

func Part1(numbers []int64) int64 {
	previous := numbers[0]
	increases := int64(0)
	for i := 1; i < len(numbers); i++ {
		number := numbers[i]
		if number > previous {
			increases++
		}
		previous = number
	}
	return increases
}

func ParseInput(lines [][]byte) []int64 {
	var numbers []int64

	for _, item := range lines {
		var number int64 = 0
		var magnitude int64 = 1
		for i := len(item) - 1; i >= 0; i-- {
			number += int64(item[i]-byte('0')) * magnitude
			magnitude *= 10
		}
		numbers = append(numbers, number)
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
