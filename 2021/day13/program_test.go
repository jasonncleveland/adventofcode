package main

import (
	"os"
	"reflect"
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("6,10"),
		[]byte("0,14"),
		[]byte("9,10"),
		[]byte("0,3"),
		[]byte("10,4"),
		[]byte("4,11"),
		[]byte("6,0"),
		[]byte("6,12"),
		[]byte("4,1"),
		[]byte("0,13"),
		[]byte("10,12"),
		[]byte("3,4"),
		[]byte("3,0"),
		[]byte("8,4"),
		[]byte("1,10"),
		[]byte("2,14"),
		[]byte("8,10"),
		[]byte("9,0"),
		[]byte(""),
		[]byte("fold along y=7"),
		[]byte("fold along x=5"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(17)
	if result != expected {
		t.Errorf("Result was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	expected := `
#####
#   #
#   #
#   #
#####
`
	lines := [][]byte{
		[]byte("6,10"),
		[]byte("0,14"),
		[]byte("9,10"),
		[]byte("0,3"),
		[]byte("10,4"),
		[]byte("4,11"),
		[]byte("6,0"),
		[]byte("6,12"),
		[]byte("4,1"),
		[]byte("0,13"),
		[]byte("10,12"),
		[]byte("3,4"),
		[]byte("3,0"),
		[]byte("8,4"),
		[]byte("1,10"),
		[]byte("2,14"),
		[]byte("8,10"),
		[]byte("9,0"),
		[]byte(""),
		[]byte("fold along y=7"),
		[]byte("fold along x=5"),
	}

	result := Part2(lines)

	if result != expected {
		t.Errorf("Result was incorrect, got: %v, want: %v.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	expectedCoordinates := []point{
		{6, 10},
		{0, 14},
		{9, 10},
		{0, 3},
		{10, 4},
		{4, 11},
		{6, 0},
		{6, 12},
		{4, 1},
		{0, 13},
		{10, 12},
		{3, 4},
		{3, 0},
		{8, 4},
		{1, 10},
		{2, 14},
		{8, 10},
		{9, 0},
	}
	expectedInstructions := []point{
		{0, 7},
		{5, 0},
	}
	lines := [][]byte{
		[]byte("6,10"),
		[]byte("0,14"),
		[]byte("9,10"),
		[]byte("0,3"),
		[]byte("10,4"),
		[]byte("4,11"),
		[]byte("6,0"),
		[]byte("6,12"),
		[]byte("4,1"),
		[]byte("0,13"),
		[]byte("10,12"),
		[]byte("3,4"),
		[]byte("3,0"),
		[]byte("8,4"),
		[]byte("1,10"),
		[]byte("2,14"),
		[]byte("8,10"),
		[]byte("9,0"),
		[]byte(""),
		[]byte("fold along y=7"),
		[]byte("fold along x=5"),
	}

	coordinates, instructions := ParseInput(lines)
	if len(coordinates) != len(expectedCoordinates) {
		t.Fatalf("Length of result is not equal to the expected length, got: %d, want %d\n", len(coordinates), len(expectedCoordinates))
	}
	if len(instructions) != len(expectedInstructions) {
		t.Fatalf("Length of result is not equal to the expected length, got: %d, want %d\n", len(instructions), len(expectedInstructions))
	}
	if !reflect.DeepEqual(coordinates, expectedCoordinates) {
		t.Errorf("Value of result not equal to expected got: %v, want %v\n", coordinates, expectedCoordinates)
	}
	if !reflect.DeepEqual(instructions, expectedInstructions) {
		t.Errorf("Value of result not equal to expected got: %v, want %v\n", instructions, expectedInstructions)
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
