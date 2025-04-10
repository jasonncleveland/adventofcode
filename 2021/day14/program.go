package day14

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
	template, rules := ParseInput(lines)

	cache := map[string]map[byte]int64{}
	occurences := map[byte]int64{}
	for index := 1; index < len(template); index++ {
		pair := template[index-1 : index+1]
		result := GeneratePolymerRec(rules, pair, 10, cache)
		for key, value := range result {
			occurences[key] += value
		}
	}

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
	template, rules := ParseInput(lines)

	cache := map[string]map[byte]int64{}
	occurences := map[byte]int64{}
	for index := 1; index < len(template); index++ {
		pair := template[index-1 : index+1]
		result := GeneratePolymerRec(rules, pair, 40, cache)
		for key, value := range result {
			occurences[key] += value
		}
	}

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

func GeneratePolymerRec(rules map[string]byte, pair []byte, step int, cache map[string]map[byte]int64) map[byte]int64 {
	if step == 0 {
		return map[byte]int64{}
	}

	cacheKey := fmt.Sprintf("%s-%d", pair, step)
	if cache[cacheKey] != nil {
		return cache[cacheKey]
	}

	next := rules[string(pair)]
	left := GeneratePolymerRec(rules, []byte{pair[0], next}, step-1, cache)
	right := GeneratePolymerRec(rules, []byte{next, pair[1]}, step-1, cache)

	copy := map[byte]int64{
		next: 1,
	}
	for key, value := range left {
		copy[key] += value
	}
	for key, value := range right {
		copy[key] += value
	}
	cache[cacheKey] = copy
	return copy
}

func ParseInput(lines [][]byte) ([]byte, map[string]byte) {
	polymerTemplate := lines[0]
	rules := map[string]byte{}

	// Parse rules
	for index := 2; index < len(lines); index++ {
		line := lines[index]
		lineParts := bytes.Split(line, []byte(" -> "))
		rules[string(lineParts[0])] = lineParts[1][0]
	}

	return polymerTemplate, rules
}
