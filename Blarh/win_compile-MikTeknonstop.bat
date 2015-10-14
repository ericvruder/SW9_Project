:: et script til at compile pdf filer, fra main.tex til main-%date% (system datoen
:: på dit system) der er også et antal linjer som sletter alle de små filer
:: pdflatex skaber når den bliver kørt.
:: Filen kan sættes til at køre sletningen alene, eller til at beholde filerne.
:: TODO:
:: --help/-h/-? kommandoer dog indeholder filen stadig kun meget lidt og skulle være til at læse så problemet er ikke stort.
:: tjek om main.tex faktisk findes...
:: brug andre filer -f/--file
@echo off

if "%1" == "-c" goto :cleanup
if "%1" == "--cleanup" goto :cleanup

pdflatex rapport.tex -job-name=main-%date% -interaction=nonstopmode
:: bibtex skal køres to gange, på filen af uvisse grunde.
bibtex main-%date%
bibtex main-%date%
pdflatex rapport.tex -job-name=main-%date% -interaction=nonstopmode
bibtex main-%date%
pdflatex rapport.tex -job-name=main-%date% -interaction=nonstopmode

if "%1" == "-nc" goto :END
if "%1" == "--nocleanup" goto :END

:cleanup
del /F *.out
del /F *.aux
del /F *.bbl
del /F *.blg
del /F *.txt
del /F *.synctex
del /F *.toc
del /F *.log
del /F *.gz
del /F *.gz(busy)

del /S "*.bbl"
del /S "*.blg"
del /S "*.log"
del /S "*.out"
del /S "*.toc"
del /S "*.aux"
del /S "*.lox"
del /S "*.tps"
del /S "*.tdo"
del /S "*.synctex.gz"
del /S "*.gz*"
del /S "*.sty"


:END