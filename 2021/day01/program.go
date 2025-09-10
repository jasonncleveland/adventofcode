package day01

import (
	"fmt"
	"time"

	"advent-of-code-2021/utils"
)

func Run(fileName string) {
	var start time.Time = time.Now()
	lines := utils.ReadFileLines(fileName)
	numbers := ParseInput(lines)
	fmt.Printf("File read: %s\n", time.Since(start))

	start = time.Now()
	part1 := Part1(numbers)
	fmt.Printf("Part 1: %d (%s)\n", part1, time.Since(start))

	start = time.Now()
	part2 := Part2(numbers)
	fmt.Printf("Part 2: %d (%s)\n", part2, time.Since(start))
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

func Part2(numbers []int64) int64 {
	previous := numbers[2] + numbers[1] + numbers[0]
	increases := int64(0)
	for i := 3; i < len(numbers); i++ {
		number := numbers[i] + numbers[i-1] + numbers[i-2]
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
