package main

import (
	"fmt"
	"os"
	"path/filepath"
	"time"

	"github.com/jasonncleveland/adventofcode/template/golang/dayTemplate"
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

	if runAllDays || dayToRun == "dayTemplate" {
		fmt.Println("Day Template")
		dayTemplate.Run(filepath.Join("dayTemplate", fileName))
	}

	fmt.Printf("Total: %s\n", time.Since(start))
}
