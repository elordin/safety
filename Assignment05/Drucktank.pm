dtmc

// constants
const int FillTimeout = 60;
const int Timeout = FillTimeout + 4;
const int RuptureTime = 5;
const int DrainTime = 10;

const double p_low = 0.0001;
const double p_high = 0.00003;

const double p_fs1 = p_high;
const double p_fk1 = p_high;
const double p_fk2 = p_high;
const double p_fs = p_low;
const double p_ft = p_low;

// d states
const int d_Empty = 0;
const int d_Filling = 1;
const int d_Filled = 2;
const int d_Ruptured = 3;
const int d_Draining = 4;

// m states
const int m_Off = 0;
const int m_On = 1;

// s1 states
const int s1_Start = 0;
const int s1_Closed = 1;
const int s1_Open = 2; 

// s states
const int s_Closed = 0;
const int s_Open = 1;

// k1 states
const int k1_Open = 0;
const int k1_Closed = 1;

// k2 states
const int k2_Open = 0;
const int k2_Closed = 1;

// t states
const int t_Closed = 0;
const int t_Countdown = 1;
const int t_Open = 2;

// failure states
const int f_no = 0;
const int f_yes = 1;

module S1
	s1 : [0..2] init s1_Start;

	// commands s1
	[synch] s1=s1_Start -> 0.5 : (s1'=s1_Start) +
			  0.5 : (s1'=s1_Closed);
	[synch] s1=s1_Open & fs1!=f_no -> 1.0 : (s1'=s1_Closed);
	[synch] s1=s1_Closed & fs1=f_no -> 1.0 : (s1'=s1_Open);
	// else
	[synch] !(s1=s1_Start | (s1=s1_Open & fs1!=f_no) | s1=s1_Closed & fs1=f_no) -> 1.0 : (s1'=s1);

endmodule

module T
	t  : [0..2] init t_Closed;

	// commands t
	[synch] t=t_Closed & s=s_Closed & (s1=s1_Closed | k1=k1_Closed) -> 1.0 : (t'=t_Countdown);
	[synch] t=t_Countdown & (s=s_Open | (s1!=s1_Closed & k1=k1_Open)) -> 1.0 : (t'=t_Closed);
	[synch] t=t_Countdown & var_t<Timeout & !(t=t_Countdown & (s=s_Open | (s1!=s1_Closed & k1=k1_Open))) -> 1.0 : (t'=t_Countdown);
	[synch] t=t_Countdown & var_t=Timeout & ft=f_no & !(t=t_Countdown & (s=s_Open | (s1!=s1_Closed & k1=k1_Open))) -> 1.0 : (t'=t_Open);
	[synch] t=t_Countdown & ft=f_yes & !(t=t_Countdown & (s=s_Open | (s1!=s1_Closed & k1=k1_Open))) & !(t=t_Countdown & var_t<Timeout & !(t=t_Countdown & (s=s_Open | (s1!=s1_Closed & k1=k1_Open)))) -> 1.0 : (t'=t_Closed);
	[synch] t=t_Open & (s=s_Open | (s1!=s1_Closed & k1=k1_Open)) -> 1.0 : (t'=t_Closed);
	//else
	[synch] !((t=t_Closed & s=s_Closed & (s1=s1_Closed | k1=k1_Closed)) |
		  (t=t_Countdown & (s=s_Open | (s1!=s1_Closed & k1=k1_Open))) |
		  (t=t_Countdown & var_t<Timeout) |
		  (t=t_Countdown & var_t=Timeout & ft=f_no) |
		  (t=t_Countdown & ft=f_yes) |
		  (t=t_Open & (s=s_Open | (s1!=s1_Closed & k1=k1_Open)))) -> 1.0 : (t'=t);

endmodule

module  varT
	var_t : [0..Timeout] init 0;

	// commands var_t
	[synch] t=t_Closed -> 1.0 : (var_t'=0);
	[synch] t=t_Countdown & var_t<Timeout -> 1.0 : (var_t'=var_t+1);
	//else
	[synch] !(t=t_Closed | (t=t_Countdown & var_t<Timeout)) -> 1.0 : (var_t'=var_t);

endmodule

module K1
	k1 : [0..1] init k1_Open;

	// commands k1
	[synch] k1=k1_Closed & t=t_Open & fk1=f_no -> 1.0 : (k1'=k1_Open);
	[synch] k1=k1_Open & t!=t_Open & s1=s1_Closed -> 1.0 : (k1'=k1_Closed);
	// else
	[synch] !((k1=k1_Closed & t=t_Open & fk1=f_no) |
		  (k1=k1_Open & t!=t_Open & s1=s1_Closed)) -> 1.0 : (k1'=k1);

endmodule

module K2
	k2 : [0..1] init k2_Open;

	// commands k2
	[synch] k2=k2_Closed & (s=s_Open | (s1!=s1_Closed & k1=k1_Open)) & fk2=f_no -> 1.0 : (k2'=k2_Open);
	[synch] k2=k2_Open & (s=s_Closed & (s1=s1_Closed | k1=k1_Closed)) -> 1.0 : (k2'=k2_Closed);
	// else
	[synch] !((k2=k2_Closed & (s=s_Open | (s1!=s1_Closed & k1=k1_Open)) & fk2=f_no) |
		  (k2=k2_Open & (s=s_Closed & (s1=s1_Closed | k1=k1_Closed)))) -> 1.0 : (k2'=k2);

endmodule

module M
	m  : [0..1] init m_Off;

	// commands m
	[synch] m=m_Off & k2=k2_Closed -> 1.0 : (m'=m_On);
	[synch] m=m_On & k2=k2_Open -> 1.0 : (m'=m_Off);
	// else
	[synch] !((m=m_Off & k2=k2_Closed) |
		  (m=m_On & k2=k2_Open)) -> 1.0 : (m'=m);

endmodule

module _S
	s  : [0..1] init s_Closed;

	// commands s
	[synch] s=s_Closed & d=d_Filled & fs=f_no -> 1.0 : (s'=s_Open);
	[synch] s=s_Open & d=d_Empty -> 1.0 : (s'=s_Closed);
	// else
	[synch] !((s=s_Closed & d=d_Filled & fs=f_no) |
		  (s=s_Open & d=d_Empty)) -> 1.0 : (s'=s);

endmodule

module D
	d  : [0..4] init d_Empty;

	// commands d
	[synch] d=d_Empty & m=m_On -> 1.0 : (d'=d_Filling);
	[synch] d=d_Filling & m=m_On & var_f<FillTimeout -> 1.0 : (d'=d_Filling);
	[synch] d=d_Filling & m=m_On & var_f=FillTimeout -> 1.0 : (d'=d_Filled);
	[synch] d=d_Filled & m=m_Off -> 1.0 : (d'=d_Draining);
	[synch] d=d_Draining & m=m_Off & var_f<DrainTime -> 1.0 : (d'=d_Draining);
	[synch] d=d_Draining & m=m_Off & var_f=DrainTime -> 1.0 : (d'=d_Empty);
	[synch] d=d_Filled & m=m_On & var_f<RuptureTime -> 1.0 : (d'=d_Filled);
	[synch] d=d_Filled & m=m_On & var_f=RuptureTime -> 1.0 : (d'=d_Ruptured);
	// else
	[synch] !((d=d_Empty & m=m_On) |
		  (d=d_Filling & m=m_On & var_f<FillTimeout) |
		  (d=d_Filling & m=m_On & var_f=FillTimeout) |
		  (d=d_Filled & m=m_Off) |
		  (d=d_Draining & m=m_Off & var_f<DrainTime) |
		  (d=d_Draining & m=m_Off & var_f=DrainTime) |
		  (d=d_Filled & m=m_On & var_f<RuptureTime) |
		  (d=d_Filled & m=m_On & var_f=RuptureTime)) -> 1.0 : (d'=d);
endmodule

module varF
	var_f : [0..FillTimeout] init 0;
	firstDraining : bool init true;
	firstFilled: bool init true;

	// commands var_f
	[synch] d=d_Filling & m=m_On & var_f<FillTimeout -> 1.0 : (var_f'=var_f+1);
	[synch] d=d_Filled & m=m_On & var_f<RuptureTime & !firstFilled -> 1.0 : (var_f'=var_f+1);
	[synch] d=d_Draining & m=m_Off & var_f<DrainTime & !firstDraining -> 1.0 : (var_f'=var_f+1);
	[synch] d=d_Empty -> 1.0 : (var_f'=0) & (firstDraining'=true) & (firstFilled'=true);
	[synch] d=d_Filled & firstFilled -> 1.0 : (var_f'=0) & (firstFilled'=false);
	[synch] d=d_Draining & firstDraining -> 1.0 : (var_f'=0) & (firstDraining'=false);
	// else
	[synch] !((d=d_Filling & m=m_On & var_f<FillTimeout) |
		  (d=d_Filled & m=m_On & var_f<RuptureTime & !firstFilled) |
		  (d=d_Draining & m=m_Off & var_f<DrainTime & !firstDraining) |
		  (d=d_Empty) |
		  (d=d_Filled & firstFilled ) |
		  (d=d_Draining & firstDraining)) -> 1.0 : (var_f'=var_f);

endmodule

module FS1
	fs1 : [0..1] init f_no;

	// commands fs1
	[synch] fs1=f_no -> p_fs1 : (fs1'=f_yes) +
		       1.0 - p_fs1 : (fs1'=f_no);
	[synch] fs1=f_yes -> p_fs1 : (fs1'=f_yes) +
		       1.0 - p_fs1 : (fs1'=f_no);

endmodule

module FS
	fs  : [0..1] init f_no;

	// commands fs
	[synch] fs=f_no -> p_fs : (fs'=f_yes) + 
		       1.0 - p_fs : (fs'=f_no);
	[synch] fs=f_yes -> 1.0 : (fs'=fs);

endmodule

module FK1
	fk1 : [0..1] init f_no;

	// commands fk1
	[synch] fk1=f_no -> p_fk1 : (fk1'=f_yes) + 
		       1.0 - p_fk1 : (fk1'=f_no);
	[synch] fk1=f_yes -> 1.0 : (fk1'=fk1);

endmodule

module FK2
	fk2 : [0..1] init f_no;

	// commands fk2		
	[synch] fk2=f_no -> p_fk2 : (fk2'=f_yes) + 
		       (1 - p_fk2 ) : (fk2'=f_no);
	[synch] fk2=f_yes -> 1.0 : (fk2'=fk2);

endmodule

module FT
	ft  : [0..1] init f_no;

	// commands ft
	[synch] ft=f_no -> p_ft : (ft'=f_yes) + 
		       1.0 - p_ft : (ft'=f_no);
	[synch] ft=f_yes -> 1.0 : (ft'=ft);

endmodule