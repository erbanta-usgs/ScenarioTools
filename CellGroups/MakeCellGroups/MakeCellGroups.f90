program MakeCellGroups

! This will process a zone-array file generated by ModelMuse.
! It assumes each zone array contains "1" for true and "0"
! for false ... only 1's and 0's are expected.

implicit none
integer :: i, iout, iu, j, m, nrow, ncol, nzones
integer, allocatable, dimension(:,:) :: zones
character(len=200) :: infile, line, outfile
character(len=10) :: zonename

100 format(i5,',',i5)
! If a line containing NROW and NCOL has been entered at top
! of ZONE file, enter filename.  
!===========================================================
nrow = -1
ncol = -1
iu = 8
iout = 9
outfile = 'CellGroups.csv'
open(iout,file=outfile,status='REPLACE')
!
write(*,*)'If zone file has NROW NCOL added as first line, omit'
write(*,*)'NROW and NCOL from response'
write(*,*)
write(*,*) 'Enter filename [NROW NCOL]:'
read(*,'(a200)')line
read(line,'(a,i,i)',err=20)infile,nrow,ncol
open(file=infile,unit=iu)!,err=900)
if (nrow==0 .or. ncol==0) then
  goto 20
endif
if (nrow<1 .or. ncol<1) then
  goto 20
endif
goto 30 ! No need to read NROW, NCOL from file

! Read error diverts here--Need to read NROW NCOL from 1st line in file
20 continue
read(iu,*)nrow,ncol

30 continue
allocate(zones(ncol,nrow))

! skip comment lines at top of zone file
40 continue
read(iu,'(a200)')line
if (line(1:1)=='#') goto 40
read(line,*)nzones

! process all zones
do m=1,nzones
  read(iu,*)zonename
  read(iu,'(A200)')line ! Ignore array control record.  Assume free format
  
  ! Read one array of zone numbers
  do i=1,nrow
    read(iu,*)(zones(j,i),j=1,ncol)
  enddo
  !
  ! process zone array and write a cell group
  write(iout,'(a10)')zonename
  do i=1,nrow
    do j=1,ncol
      if (zones(j,i)==1) then
        write(iout,100)i,j
      endif
    enddo
  enddo
  !
enddo

close(iu)
close(iout)
write(iout,*)'File "',trim(outfile),'" contains the cell groups'

stop 'Normal termination'
!===========================================================
! Error handling
900 continue
write(*,*)'Error encountered'
end program MakeCellGroups