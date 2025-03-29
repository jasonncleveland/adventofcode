package main

import (
	"testing"
)

func TestPart1(t *testing.T) {
	bits := [][]int64{
		{0, 0, 1, 0, 0},
		{1, 1, 1, 1, 0},
		{1, 0, 1, 1, 0},
		{1, 0, 1, 1, 1},
		{1, 0, 1, 0, 1},
		{0, 1, 1, 1, 1},
		{0, 0, 1, 1, 1},
		{1, 1, 1, 0, 0},
		{1, 0, 0, 0, 0},
		{1, 1, 0, 0, 1},
		{0, 0, 0, 1, 0},
		{0, 1, 0, 1, 0},
	}
	result := Part1(bits)

	expected := int64(198)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	bits := [][]int64{
		{0, 0, 1, 0, 0},
		{1, 1, 1, 1, 0},
		{1, 0, 1, 1, 0},
		{1, 0, 1, 1, 1},
		{1, 0, 1, 0, 1},
		{0, 1, 1, 1, 1},
		{0, 0, 1, 1, 1},
		{1, 1, 1, 0, 0},
		{1, 0, 0, 0, 0},
		{1, 1, 0, 0, 1},
		{0, 0, 0, 1, 0},
		{0, 1, 0, 1, 0},
	}
	result := Part2(bits)

	expected := int64(230)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	bits := [][]int64{
		{0, 0, 1, 0, 0},
		{1, 1, 1, 1, 0},
		{1, 0, 1, 1, 0},
		{1, 0, 1, 1, 1},
		{1, 0, 1, 0, 1},
		{0, 1, 1, 1, 1},
		{0, 0, 1, 1, 1},
		{1, 1, 1, 0, 0},
		{1, 0, 0, 0, 0},
		{1, 1, 0, 0, 1},
		{0, 0, 0, 1, 0},
		{0, 1, 0, 1, 0},
	}
	lines := [][]byte{
		[]byte("00100"),
		[]byte("11110"),
		[]byte("10110"),
		[]byte("10111"),
		[]byte("10101"),
		[]byte("01111"),
		[]byte("00111"),
		[]byte("11100"),
		[]byte("10000"),
		[]byte("11001"),
		[]byte("00010"),
		[]byte("01010"),
	}

	result := ParseInput(lines)
	if len(result) != len(bits) {
		t.Fatalf("Length of result %d is not equal to the expected length %d\n", len(result), len(bits))
	}
	for i := range result {
		if len(result[i]) != len(bits[i]) {
			t.Fatalf("Length of result %d is not equal to the expected length %d\n", len(result[i]), len(bits[i]))
		}
		for j := range result[i] {
			if result[i][j] != bits[i][j] {
				t.Errorf("Number at index %d in result %d is not equal to expected %d\n", i, result[i][j], bits[i][j])
			}
		}
	}
}

func TestReadFileLines(t *testing.T) {
	lines := [][]byte{
		[]byte("00100"),
		[]byte("11110"),
		[]byte("10110"),
		[]byte("10111"),
		[]byte("10101"),
		[]byte("01111"),
		[]byte("00111"),
		[]byte("11100"),
		[]byte("10000"),
		[]byte("11001"),
		[]byte("00010"),
		[]byte("01010"),
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
