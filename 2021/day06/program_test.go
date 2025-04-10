package day06

import (
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("3,4,3,1,2"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(5934)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		[]byte("3,4,3,1,2"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(26984457539)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	numbers := [][]int64{
		{3, 4, 3, 1, 2},
	}
	lines := [][]byte{
		[]byte("3,4,3,1,2"),
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
