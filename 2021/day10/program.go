package main

import (
	"bytes"
	"container/list"
	"fmt"
	"os"
	"slices"
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
	scores := map[byte]int64{
		byte(')'): 3,
		byte(']'): 57,
		byte('}'): 1197,
		byte('>'): 25137,
	}
	expected := map[byte]byte{
		byte(')'): byte('('),
		byte(']'): byte('['),
		byte('}'): byte('{'),
		byte('>'): byte('<'),
	}
	stack := list.New()
	total := int64(0)
	for _, line := range lines {
		for _, character := range line {
			if character == byte('(') || character == byte('[') || character == byte('{') || character == byte('<') {
				stack.PushBack(character)
			} else {
				if expected[character] == stack.Back().Value.(byte) {
					stack.Remove(stack.Back())
				} else {
					total += scores[character]
					break
				}
			}
		}
	}
	return total
}

func Part2(lines [][]byte) int64 {
	scores := map[byte]int64{
		byte(')'): 1,
		byte(']'): 2,
		byte('}'): 3,
		byte('>'): 4,
	}
	expected := map[byte]byte{
		byte(')'): byte('('),
		byte('('): byte(')'),
		byte(']'): byte('['),
		byte('['): byte(']'),
		byte('}'): byte('{'),
		byte('{'): byte('}'),
		byte('>'): byte('<'),
		byte('<'): byte('>'),
	}
	var totals []int64
	for _, line := range lines {
		stack := list.New()
		isValid := true
		for _, character := range line {
			if character == byte('(') || character == byte('[') || character == byte('{') || character == byte('<') {
				stack.PushBack(character)
			} else {
				if expected[character] == stack.Back().Value.(byte) {
					stack.Remove(stack.Back())
				} else {
					// Ignore invalid lines
					isValid = false
					break
				}
			}
		}
		// Fix incomplete lines
		if isValid && stack.Len() > 0 {
			total := int64(0)
			for stack.Len() > 0 {
				total = total*5 + scores[expected[stack.Back().Value.(byte)]]
				stack.Remove(stack.Back())
			}
			totals = append(totals, total)
		}
	}
	slices.Sort(totals)
	return totals[len(totals)/2]
}

func ReadFileLines(fileName string) [][]byte {
	data, err := os.ReadFile(fileName)
	if err != nil {
		panic(err)
	}
	lines := bytes.Split(data, []byte("\n"))

	return lines
}
