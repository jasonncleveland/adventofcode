package day08

import (
	"bytes"
	"fmt"
	"slices"
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
	entries := ParseInput(lines)

	total := int64(0)
	for _, entry := range entries {
		for _, output := range entry.outputs {
			if len(output) == 2 || len(output) == 4 || len(output) == 3 || len(output) == 7 {
				total++
			}
		}
	}
	return total
}

func Part2(lines [][]byte) int64 {
	entries := ParseInput(lines)

	total := int64(0)
	for _, entry := range entries {
		/**
		 * Display:
		 *  1111
		 * 2    3
		 * 2    3
		 *  4444
		 * 5    6
		 * 5    6
		 *  7777
		 */
		display := FindConnections(entry)
		digits := CreateDisplayMap(display)

		// Start from the right-most number
		slices.Reverse(entry.outputs)

		number := 0
		magnitude := 1
		for _, output := range entry.outputs {
			slices.Sort(output)
			number += digits[string(output)] * magnitude
			magnitude *= 10
		}
		total += int64(number)
	}
	return total
}

func ParseInput(lines [][]byte) []entry {
	var entries []entry

	for _, line := range lines {
		lineParts := bytes.Split(line, []byte(" | "))
		inputs := bytes.Split(lineParts[0], []byte(" "))
		outputs := bytes.Split(lineParts[1], []byte(" "))
		entries = append(entries, entry{
			inputs,
			outputs,
		})
	}

	return entries
}

func FindDiff(first []byte, second []byte) []byte {
	occurences := make(map[byte]int)

	for _, item := range first {
		occurences[item]++
	}

	for _, item := range second {
		occurences[item]++
	}

	var differences []byte
	for item, count := range occurences {
		if count == 1 {
			differences = append(differences, item)
		}
	}
	return differences
}

func FindConnections(entry entry) map[int]byte {
	/**
	 * Display:
	 *  1111
	 * 2    3
	 * 2    3
	 *  4444
	 * 5    6
	 * 5    6
	 *  7777
	 */
	var display map[int]byte = map[int]byte{
		1: byte(0),
		2: byte(0),
		3: byte(0),
		4: byte(0),
		5: byte(0),
		6: byte(0),
		7: byte(0),
	}
	var groups map[int][][]byte = map[int][][]byte{
		2: {},
		3: {},
		4: {},
		5: {},
		6: {},
		7: {},
	}

	// Group inputs by number of segments
	for _, input := range entry.inputs {
		slices.Sort(input)
		switch len(input) {
		case 2:
			groups[2] = append(groups[2], input)
		case 3:
			groups[3] = append(groups[3], input)
		case 4:
			groups[4] = append(groups[4], input)
		case 5:
			groups[5] = append(groups[5], input)
		case 6:
			groups[6] = append(groups[6], input)
		case 7:
			groups[7] = append(groups[7], input)
		default:
			fmt.Println("found unexpected number of segments:", input)
			panic("Found invalid number of segments")
		}
	}

	// The top segment is the unique segment between "1" and "7"
	display[1] = FindDiff(groups[2][0], groups[3][0])[0]

	// Find the middle segment
	for _, possibleSegment := range FindDiff(groups[2][0], groups[4][0]) {
		occurences := 0
		for _, input := range groups[6] {
			for _, segment := range input {
				if possibleSegment == segment {
					occurences++
				}
			}
		}
		if occurences == len(groups[6]) {
			// The top left segment is the segment that is in all of "0", "6", and "9"
			display[2] = possibleSegment
		} else {
			// The middle segment is the segment that is not in all of "0", "6", and "9"
			display[4] = possibleSegment
		}
	}

	// Find bottom left and top right segments
	for _, input := range groups[6] {
		possibleSegments := FindDiff(input, groups[7][0])
		if possibleSegments[0] != display[4] {
			if slices.Contains(groups[2][0], possibleSegments[0]) {
				// The top right segment is the segment that is found in "1"
				display[3] = possibleSegments[0]
			} else {
				// The bottom left segment is the segment that is not found in "1"
				display[5] = possibleSegments[0]
			}
		}
	}

	// The bottom right segment is the unknown segment from "1"
	display[6] = FindDiff(groups[2][0], []byte{display[3]})[0]

	// The bottom segment is the unknown segment from "8"
	for _, possibleSegment := range groups[7][0] {
		isFound := false
		for _, segment := range display {
			if possibleSegment == segment {
				isFound = true
				break
			}
		}
		if !isFound {
			display[7] = possibleSegment
		}
	}

	return display
}

func CreateDisplayMap(display map[int]byte) map[string]int {

	// 0
	zero := []byte{display[1], display[2], display[3], display[5], display[6], display[7]}
	slices.Sort(zero)
	// 1
	one := []byte{display[3], display[6]}
	slices.Sort(one)
	// 2
	two := []byte{display[1], display[3], display[4], display[5], display[7]}
	slices.Sort(two)
	// 3
	three := []byte{display[1], display[3], display[4], display[6], display[7]}
	slices.Sort(three)
	// 4
	four := []byte{display[2], display[3], display[4], display[6]}
	slices.Sort(four)
	// 5
	five := []byte{display[1], display[2], display[4], display[6], display[7]}
	slices.Sort(five)
	// 6
	six := []byte{display[1], display[2], display[4], display[5], display[6], display[7]}
	slices.Sort(six)
	// 7
	seven := []byte{display[1], display[3], display[6]}
	slices.Sort(seven)
	// 8
	eight := []byte{display[1], display[2], display[3], display[4], display[5], display[6], display[7]}
	slices.Sort(eight)
	// 9
	nine := []byte{display[1], display[2], display[3], display[4], display[6], display[7]}
	slices.Sort(nine)

	return map[string]int{
		string(zero):  0,
		string(one):   1,
		string(two):   2,
		string(three): 3,
		string(four):  4,
		string(five):  5,
		string(six):   6,
		string(seven): 7,
		string(eight): 8,
		string(nine):  9,
	}
}

type entry struct {
	inputs  [][]byte
	outputs [][]byte
}
