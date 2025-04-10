package day07

import (
	"testing"
)

func TestPart1(t *testing.T) {
	lines := [][]byte{
		[]byte("16,1,2,0,4,2,7,1,2,14"),
	}

	result := Part1(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 1 not implemented")
	}

	expected := int64(37)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestPart2(t *testing.T) {
	lines := [][]byte{
		[]byte("16,1,2,0,4,2,7,1,2,14"),
	}

	result := Part2(lines)
	if result == -1 {
		t.Fatal("Function or tests for part 2 not implemented")
	}

	expected := int64(168)
	if result != expected {
		t.Errorf("Error was incorrect, got: %d, want: %d.\n", result, expected)
	}
}

func TestParseInput(t *testing.T) {
	numbers := [][]int64{
		{16, 1, 2, 0, 4, 2, 7, 1, 2, 14},
	}
	lines := [][]byte{
		[]byte("16,1,2,0,4,2,7,1,2,14"),
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
