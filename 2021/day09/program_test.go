package main

import (
	"os"
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("2199943210"),
		[]byte("3987894921"),
		[]byte("9856789892"),
		[]byte("8767896789"),
		[]byte("9899965678"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(15)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		[]byte("2199943210"),
		[]byte("3987894921"),
		[]byte("9856789892"),
		[]byte("8767896789"),
		[]byte("9899965678"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(1134)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	numbers := [][]int64{
		{0, 0, 0, 0, 0},
		{0, 0, 1, 0, 0},
		{0, 1, 1, 1, 0},
		{0, 0, 1, 0, 0},
		{0, 0, 0, 0, 0},
	}
	lines := [][]byte{
		[]byte("00000"),
		[]byte("00100"),
		[]byte("01110"),
		[]byte("00100"),
		[]byte("00000"),
	}

	result := ParseInput(lines)
	if len(result) != len(numbers) {
		t.Fatalf("Length of result %d is not equal to the expected length %d\n", len(result), len(numbers))
	}
	for i := range result {
		if len(result[i]) != len(numbers[i]) {
			t.Fatalf("Length of result %d is not equal to the expected length %d\n", len(result[i]), len(numbers[i]))
		}
		for j := range result[i] {
			if result[i][j] != numbers[i][j] {
				t.Errorf("Number at index %d in result %d is not equal to expected %d\n", i, result[i][j], numbers[i][j])
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
