package main

import (
	"os"
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("5483143223"),
		[]byte("2745854711"),
		[]byte("5264556173"),
		[]byte("6141336146"),
		[]byte("6357385478"),
		[]byte("4167524645"),
		[]byte("2176841721"),
		[]byte("6882881134"),
		[]byte("4846848554"),
		[]byte("5283751526"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(1656)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		[]byte("00000"),
		[]byte("00100"),
		[]byte("01110"),
		[]byte("00100"),
		[]byte("00000"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(0)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	expected := [][]int64{
		{5, 4, 8, 3, 1, 4, 3, 2, 2, 3},
		{2, 7, 4, 5, 8, 5, 4, 7, 1, 1},
		{5, 2, 6, 4, 5, 5, 6, 1, 7, 3},
		{6, 1, 4, 1, 3, 3, 6, 1, 4, 6},
		{6, 3, 5, 7, 3, 8, 5, 4, 7, 8},
		{4, 1, 6, 7, 5, 2, 4, 6, 4, 5},
		{2, 1, 7, 6, 8, 4, 1, 7, 2, 1},
		{6, 8, 8, 2, 8, 8, 1, 1, 3, 4},
		{4, 8, 4, 6, 8, 4, 8, 5, 5, 4},
		{5, 2, 8, 3, 7, 5, 1, 5, 2, 6},
	}
	lines := [][]byte{
		[]byte("5483143223"),
		[]byte("2745854711"),
		[]byte("5264556173"),
		[]byte("6141336146"),
		[]byte("6357385478"),
		[]byte("4167524645"),
		[]byte("2176841721"),
		[]byte("6882881134"),
		[]byte("4846848554"),
		[]byte("5283751526"),
	}

	result := ParseInput(lines)
	if len(result) != len(expected) {
		t.Fatalf("Length of result is not equal to the expected length, got: %d, want %d\n", len(result), len(expected))
	}
	for i := range result {
		if len(result[i]) != len(expected[i]) {
			t.Fatalf("Length of result is not equal to the expected length, got: %d, want %d\n", len(result[i]), len(expected[i]))
		}
		for j := range result[i] {
			if result[i][j] != expected[i][j] {
				t.Errorf("Value at index %d in result is not equal to expected, got: %d, want: %d\n", i, result[i][j], expected[i][j])
			}
		}
	}
}

func TestReadFileLines(t *testing.T) {
	line := []byte("#######\n#.....#\n#..#..#\n#.###.#\n#.###.#\n#..#..#\n#.....#\n#######")
	lines := [][]byte{
		[]byte("#######"),
		[]byte("#.....#"),
		[]byte("#..#..#"),
		[]byte("#.###.#"),
		[]byte("#.###.#"),
		[]byte("#..#..#"),
		[]byte("#.....#"),
		[]byte("#######"),
	}

	tempDir := t.TempDir()
	fileName := tempDir + "testFileName"
	err := os.WriteFile(fileName, line, 0777)
	if err != nil {
		t.Fatal("Could not write test file with name", fileName)
	}

	// Test with a valid file
	result := ReadFileLines(fileName)
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
