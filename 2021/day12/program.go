package main

import (
	"bytes"
	"container/list"
	"fmt"
	"os"
	"slices"
	"strings"
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
	caves := ParseInput(lines)

	return TraverseCaves(caves)
}

func Part2(lines [][]byte) int64 {
	return -1
}

func ParseInput(lines [][]byte) map[string][]string {
	data := make(map[string][]string)

	for _, line := range lines {
		lineParts := bytes.Split(line, []byte("-"))
		data[string(lineParts[0])] = append(data[string(lineParts[0])], string(lineParts[1]))
		data[string(lineParts[1])] = append(data[string(lineParts[1])], string(lineParts[0]))
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

func TraverseCaves(caves map[string][]string) int64 {
	queue := list.New()

	queue.PushBack(node{"start", []string{"start"}})

	var total int64 = 0
	for queue.Len() > 0 {
		cave := queue.Front().Value.(node)
		queue.Remove(queue.Front())

		if cave.name == "end" {
			total++
			continue
		}

		for _, next := range caves[cave.name] {
			if slices.Contains(cave.visited, next) {
				continue
			}

			visited := slices.Clone(cave.visited)
			if strings.ToLower(next) == next {
				visited = append(visited, next)
			}
			queue.PushBack(node{next, visited})
		}
	}
	return total
}

type node struct {
	name    string
	visited []string
}
