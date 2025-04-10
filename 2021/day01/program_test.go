package day01

import (
	"testing"
)

func TestPart1(t *testing.T) {
	var numbers []int64 = []int64{199, 200, 208, 210, 200, 207, 240, 269, 260, 263}
	result := Part1(numbers)

	if result != 7 {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, 7)
	}
}

func TestPart2(t *testing.T) {
	var numbers []int64 = []int64{199, 200, 208, 210, 200, 207, 240, 269, 260, 263}
	result := Part2(numbers)

	if result != 5 {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, 5)
	}
}

func TestParseInput(t *testing.T) {
	var numbers []int64 = []int64{199, 200, 208, 210, 200, 207, 240, 269, 260, 263}
	var lines [][]byte

	// Convert numbers to byte arrays
	for _, number := range numbers {
		var line []byte
		for number > 0 {
			// Prepend current number to front of array
			line = append([]byte{byte('0') + byte(number%10)}, line...)
			number /= 10
		}
		lines = append(lines, line)
	}

	result := ParseInput(lines)
	if len(result) != len(numbers) {
		t.Fatalf("Length of result %d is not equal to the expected length %d\n", len(result), len(numbers))
	}
	for i := range result {
		if result[i] != numbers[i] {
			t.Errorf("Number at index %d in result %d is not equal to expected %d\n", i, result[i], numbers[i])
		}
	}
}
