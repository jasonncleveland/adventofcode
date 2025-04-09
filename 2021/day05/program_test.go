package day05

import (
	"os"
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("0,9 -> 5,9"),
		[]byte("8,0 -> 0,8"),
		[]byte("9,4 -> 3,4"),
		[]byte("2,2 -> 2,1"),
		[]byte("7,0 -> 7,4"),
		[]byte("6,4 -> 2,0"),
		[]byte("0,9 -> 2,9"),
		[]byte("3,4 -> 1,4"),
		[]byte("0,0 -> 8,8"),
		[]byte("5,5 -> 8,2"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(5)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		[]byte("0,9 -> 5,9"),
		[]byte("8,0 -> 0,8"),
		[]byte("9,4 -> 3,4"),
		[]byte("2,2 -> 2,1"),
		[]byte("7,0 -> 7,4"),
		[]byte("6,4 -> 2,0"),
		[]byte("0,9 -> 2,9"),
		[]byte("3,4 -> 1,4"),
		[]byte("0,0 -> 8,8"),
		[]byte("5,5 -> 8,2"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(12)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	expected := []line{
		{point{0, 9}, point{5, 9}},
		{point{8, 0}, point{0, 8}},
		{point{9, 4}, point{3, 4}},
		{point{2, 2}, point{2, 1}},
		{point{7, 0}, point{7, 4}},
		{point{6, 4}, point{2, 0}},
		{point{0, 9}, point{2, 9}},
		{point{3, 4}, point{1, 4}},
		{point{0, 0}, point{8, 8}},
		{point{5, 5}, point{8, 2}},
	}
	lines := [][]byte{
		[]byte("0,9 -> 5,9"),
		[]byte("8,0 -> 0,8"),
		[]byte("9,4 -> 3,4"),
		[]byte("2,2 -> 2,1"),
		[]byte("7,0 -> 7,4"),
		[]byte("6,4 -> 2,0"),
		[]byte("0,9 -> 2,9"),
		[]byte("3,4 -> 1,4"),
		[]byte("0,0 -> 8,8"),
		[]byte("5,5 -> 8,2"),
	}

	result := ParseInput(lines)
	if len(result) != len(expected) {
		t.Fatalf("Length of result %d is not equal to the expected length %d\n", len(result), len(expected))
	}
	for i := range result {
		if result[i] != expected[i] {
			t.Errorf("Value at index %d in result %v is not equal to expected %v\n", i, result[i], expected[i])
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
