package day07

import (
	"bytes"
	"fmt"
	"math"
	"time"

	"github.com/jasonncleveland/adventofcode/2021/utils"
)

func Run(fileName string) {
	var start time.Time = time.Now()
	lines := utils.ReadFileLines(fileName)
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
		total := int64(0)
		for _, crab := range numbers {
			fuel := crab - step
			if fuel < 0 {
				// Ensure value is positive
				fuel *= -1
			}
			total += fuel

			// Exit early if we've already surpassed the min fuel found
			if total > minFuel {
				break
			}
		}
		if total < minFuel {
			minFuel = total
		}
	}

	return minFuel
}

func Part2(lines [][]byte) int64 {
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
		total := int64(0)
		for _, crab := range numbers {
			fuel := crab - step
			if fuel < 0 {
				// Ensure value is positive
				fuel *= -1
			}
			fuel = fuel * (fuel + 1) / 2
			total += fuel

			// Exit early if we've already surpassed the min fuel found
			if total > minFuel {
				break
			}
		}
		if total < minFuel {
			minFuel = total
		}
	}

	return minFuel
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
