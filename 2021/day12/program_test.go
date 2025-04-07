package main

import (
	"os"
	"reflect"
	"testing"
)

func TestPart1(t *testing.T) {
	tests := []struct {
		input    [][]byte
		expected int64
	}{
		{
			[][]byte{
				[]byte("start-A"),
				[]byte("start-b"),
				[]byte("A-c"),
				[]byte("A-b"),
				[]byte("b-d"),
				[]byte("A-end"),
				[]byte("b-end"),
			},
			10,
		},
		{

			[][]byte{
				[]byte("dc-end"),
				[]byte("HN-start"),
				[]byte("start-kj"),
				[]byte("dc-start"),
				[]byte("dc-HN"),
				[]byte("LN-dc"),
				[]byte("HN-end"),
				[]byte("kj-sa"),
				[]byte("kj-HN"),
				[]byte("kj-dc"),
			},
			19,
		},
		{
			[][]byte{
				[]byte("fs-end"),
				[]byte("he-DX"),
				[]byte("fs-he"),
				[]byte("start-DX"),
				[]byte("pj-DX"),
				[]byte("end-zg"),
				[]byte("zg-sl"),
				[]byte("zg-pj"),
				[]byte("pj-he"),
				[]byte("RW-he"),
				[]byte("fs-DX"),
				[]byte("pj-RW"),
				[]byte("zg-RW"),
				[]byte("start-pj"),
				[]byte("he-WI"),
				[]byte("zg-he"),
				[]byte("pj-fs"),
				[]byte("start-RW"),
			},
			226,
		},
	}

	for _, test := range tests {
		result := Part1(test.input)
		if result == -1 {
			t.Fatal("Function or tests for part 1 not implemented")
		}

		if result != test.expected {
			t.Errorf("Result was incorrect, got: %d, want: %d.\n", result, test.expected)
		}
	}
}

func TestPart2(t *testing.T) {
	tests := []struct {
		input    [][]byte
		expected int64
	}{
		{
			[][]byte{
				[]byte("start-A"),
				[]byte("start-b"),
				[]byte("A-c"),
				[]byte("A-b"),
				[]byte("b-d"),
				[]byte("A-end"),
				[]byte("b-end"),
			},
			36,
		},
		{

			[][]byte{
				[]byte("dc-end"),
				[]byte("HN-start"),
				[]byte("start-kj"),
				[]byte("dc-start"),
				[]byte("dc-HN"),
				[]byte("LN-dc"),
				[]byte("HN-end"),
				[]byte("kj-sa"),
				[]byte("kj-HN"),
				[]byte("kj-dc"),
			},
			103,
		},
		{
			[][]byte{
				[]byte("fs-end"),
				[]byte("he-DX"),
				[]byte("fs-he"),
				[]byte("start-DX"),
				[]byte("pj-DX"),
				[]byte("end-zg"),
				[]byte("zg-sl"),
				[]byte("zg-pj"),
				[]byte("pj-he"),
				[]byte("RW-he"),
				[]byte("fs-DX"),
				[]byte("pj-RW"),
				[]byte("zg-RW"),
				[]byte("start-pj"),
				[]byte("he-WI"),
				[]byte("zg-he"),
				[]byte("pj-fs"),
				[]byte("start-RW"),
			},
			3509,
		},
	}

	for _, test := range tests {
		result := Part2(test.input)
		if result == -1 {
			t.Fatal("Function or tests for part 2 not implemented")
		}

		if result != test.expected {
			t.Errorf("Result was incorrect, got: %d, want: %d.\n", result, test.expected)
		}
	}
}

func TestParseInput(t *testing.T) {
	expected := map[string][]string{
		"start": {"A", "b"},
		"A":     {"start", "c", "b", "end"},
		"b":     {"start", "A", "d", "end"},
		"c":     {"A"},
		"d":     {"b"},
		"end":   {"A", "b"},
	}
	lines := [][]byte{
		[]byte("start-A"),
		[]byte("start-b"),
		[]byte("A-c"),
		[]byte("A-b"),
		[]byte("b-d"),
		[]byte("A-end"),
		[]byte("b-end"),
	}

	result := ParseInput(lines)
	if !reflect.DeepEqual(result, expected) {
		t.Fatalf("Result is not equal to expected, got: %v, want %v\n", result, expected)
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
