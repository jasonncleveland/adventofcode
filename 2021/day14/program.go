package day14

import (
	"bytes"
	"fmt"
	"math"
	"strings"
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
	template, rules := ParseInput(lines)

	for range 10 {
		newTemplate := strings.Builder{}
		newTemplate.WriteByte(template[0])
		for index := 1; index < len(template); index++ {
			pair := string(template[index-1 : index+1])
			if rules[pair] != 0 {
				newTemplate.WriteByte(rules[pair])
			}
			newTemplate.WriteByte(template[index])
		}
		template = newTemplate.String()
	}

	occurences := map[byte]int64{}
	for index := range len(template) {
		occurences[template[index]]++
	}

	var max int64 = math.MinInt64
	var min int64 = math.MaxInt64
	for _, count := range occurences {
		if count > max {
			max = int64(count)
		}
		if count < min {
			min = count
		}
	}
	return max - min
}

func Part2(lines [][]byte) int64 {
	return -1
}

func ParseInput(lines [][]byte) (string, map[string]byte) {
	polymerTemplate := lines[0]
	rules := map[string]byte{}

	// Parse rules
	for index := 2; index < len(lines); index++ {
		line := lines[index]
		lineParts := bytes.Split(line, []byte(" -> "))
		rules[string(lineParts[0])] = lineParts[1][0]
	}

	return string(polymerTemplate), rules
}
