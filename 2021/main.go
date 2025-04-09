package main

import (
	"fmt"
	"os"
	"path/filepath"
	"time"

	"github.com/jasonncleveland/adventofcode/2021/day01"
	"github.com/jasonncleveland/adventofcode/2021/day02"
	"github.com/jasonncleveland/adventofcode/2021/day03"
	"github.com/jasonncleveland/adventofcode/2021/day04"
	"github.com/jasonncleveland/adventofcode/2021/day05"
	"github.com/jasonncleveland/adventofcode/2021/day06"
	"github.com/jasonncleveland/adventofcode/2021/day07"
	"github.com/jasonncleveland/adventofcode/2021/day08"
	"github.com/jasonncleveland/adventofcode/2021/day09"
	"github.com/jasonncleveland/adventofcode/2021/day10"
	"github.com/jasonncleveland/adventofcode/2021/day11"
	"github.com/jasonncleveland/adventofcode/2021/day12"
	"github.com/jasonncleveland/adventofcode/2021/day13"
)

func main() {
	if len(os.Args) < 2 {
		panic("must provide input file name as first command line argument")
	}
	fileName := os.Args[1]

	var runAllDays bool = true
	var dayToRun string
	if len(os.Args) > 2 {
		dayToRun = os.Args[2]
		runAllDays = false
	}

	var start time.Time = time.Now()

	if runAllDays || dayToRun == "day01" {
		fmt.Println("Day 01")
		day01.Run(filepath.Join("day01", fileName))
	}
	if runAllDays || dayToRun == "day02" {
		fmt.Println("Day 02")
		day02.Run(filepath.Join("day02", fileName))
	}
	if runAllDays || dayToRun == "day03" {
		fmt.Println("Day 03")
		day03.Run(filepath.Join("day03", fileName))
	}
	if runAllDays || dayToRun == "day04" {
		fmt.Println("Day 04")
		day04.Run(filepath.Join("day04", fileName))
	}
	if runAllDays || dayToRun == "day05" {
		fmt.Println("Day 05")
		day05.Run(filepath.Join("day05", fileName))
	}
	if runAllDays || dayToRun == "day06" {
		fmt.Println("Day 06")
		day06.Run(filepath.Join("day06", fileName))
	}
	if runAllDays || dayToRun == "day07" {
		fmt.Println("Day 07")
		day07.Run(filepath.Join("day07", fileName))
	}
	if runAllDays || dayToRun == "day08" {
		fmt.Println("Day 08")
		day08.Run(filepath.Join("day08", fileName))
	}
	if runAllDays || dayToRun == "day09" {
		fmt.Println("Day 09")
		day09.Run(filepath.Join("day09", fileName))
	}
	if runAllDays || dayToRun == "day10" {
		fmt.Println("Day 10")
		day10.Run(filepath.Join("day10", fileName))
	}
	if runAllDays || dayToRun == "day11" {
		fmt.Println("Day 11")
		day11.Run(filepath.Join("day11", fileName))
	}
	if runAllDays || dayToRun == "day12" {
		fmt.Println("Day 12")
		day12.Run(filepath.Join("day12", fileName))
	}
	if runAllDays || dayToRun == "day13" {
		fmt.Println("Day 13")
		day13.Run(filepath.Join("day13", fileName))
	}

	fmt.Printf("Total: %s\n", time.Since(start))
}
