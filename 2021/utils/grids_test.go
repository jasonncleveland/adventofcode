package utils

import "testing"

func TestParseIntGrid(t *testing.T) {
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

	result := ParseIntGrid(lines)
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

func TestGridIsValid(t *testing.T) {
	grid := IntGrid{
		{0, 1, 0},
		{1, 0, 1},
		{0, 1, 0},
	}

	tests := []struct {
		input    Coordinate
		expected bool
	}{
		{
			Coordinate{Row: 1, Column: 1},
			true,
		},
		{
			Coordinate{Row: -1, Column: -1},
			false,
		},
		{
			Coordinate{Row: 37, Column: 42},
			false,
		},
	}

	for _, test := range tests {
		result := grid.IsValid(test.input.Row, test.input.Column)

		if result != test.expected {
			t.Errorf("Result was not equal to expected, got: %t, want: %t.\n", result, test.expected)
		}
	}
}

func TestGridAt(t *testing.T) {
	grid := IntGrid{
		{0, 1, 0},
		{1, 0, 1},
		{0, 1, 0},
	}

	// Test valid
	expected := int64(1)
	result := grid.At(0, 1)

	if result != expected {
		t.Errorf("Result was not equal to expected, got: %d, want: %d.\n", result, expected)
	}

	// Test error
	defer func() {
		if r := recover(); r == nil {
			t.Fatal("Expected to recover panic from file open error")
		}
	}()
	grid.At(-1, -1)
}

func TestGridPrint(t *testing.T) {
	grid := IntGrid{
		{0, 1, 1, 1, 0},
		{1, 0, 1, 0, 1},
		{1, 1, 0, 1, 1},
		{1, 0, 1, 0, 1},
		{0, 1, 1, 1, 0},
	}

	grid.Print(0)
}

func TestGridDjikstra(t *testing.T) {
	grid := IntGrid{
		{1, 8, 8, 8, 8},
		{1, 8, 8, 8, 8},
		{1, 1, 1, 1, 8},
		{8, 8, 8, 1, 8},
		{8, 8, 8, 1, 1},
	}

	expected := int64(8)
	result := grid.Djikstra(Coordinate{0, 0}, Coordinate{4, 4})

	if result != expected {
		t.Errorf("Result was not equal to expected, got: %d, want: %d.\n", result, expected)
	}
}
