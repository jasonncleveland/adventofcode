package main

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

func TestReadFileLines(t *testing.T) {
	var lines [][]byte = [][]byte{
		[]byte("199"),
		[]byte("200"),
		[]byte("208"),
		[]byte("210"),
		[]byte("200"),
		[]byte("207"),
		[]byte("240"),
		[]byte("269"),
		[]byte("260"),
		[]byte("263"),
	}

	// Test with a valid file
	result := ReadFileLines("testData.txt")
	if len(result) != len(lines) {
		t.Fatalf("Length of result %d is not equal to the expected length %d\n", len(result), len(lines))
	}
	for i := range result {
		if len(result[i]) != len(lines[i]) {
			t.Fatalf("Length of result %d is not equal to the expected length %d\n", len(result[i]), len(lines[i]))
		}
		for j := range result[i] {
			if result[i][j] != lines[i][j] {
				t.Errorf("Number at index %d in result %d is not equal to expected %d\n", i, result[i][j], lines[i][j])
			}
		}
	}

	// Test with an invalid file
	defer func() {
		if r := recover(); r == nil {
			t.Fatal("Expected to recover panic from file open error")
		}
	}()
	ReadFileLines("invalidData.txt")
}
