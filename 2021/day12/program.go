package day12

import (
	"bytes"
	"container/list"
	"fmt"
	"os"
	"slices"
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
	caves := ParseInput(lines)

	return TraverseCaves(caves, false)
}

func Part2(lines [][]byte) int64 {
	caves := ParseInput(lines)

	return TraverseCaves(caves, true)
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

func TraverseCaves(caves map[string][]string, allowSingleDuplicate bool) int64 {
	queue := list.New()

	queue.PushBack(node{"start", []string{"start"}, false})

	var total int64 = 0
	for queue.Len() > 0 {
		cave := queue.Front().Value.(node)
		queue.Remove(queue.Front())

		if cave.name == "end" {
			total++
			continue
		}

		for _, next := range caves[cave.name] {
			if next == "start" {
				continue
			}

			duplicateFound := cave.duplicateFound
			if slices.Contains(cave.visited, next) {
				if allowSingleDuplicate && !duplicateFound {
					duplicateFound = true
				} else {
					continue
				}
			}

			visited := slices.Clone(cave.visited)
			if next > "Z" {
				visited = append(visited, next)
			}
			queue.PushBack(node{next, visited, duplicateFound})
		}
	}
	return total
}

type node struct {
	name           string
	visited        []string
	duplicateFound bool
}
