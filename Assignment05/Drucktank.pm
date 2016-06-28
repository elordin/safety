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

module drucktank
	d  : [0..4] init d_Empty;
	m  : [0..1] init m_Off;
	s1 : [0..3] init s1_Start;
	s  : [0..1] init s_Closed;
	k1 : [0..1] init k1_Open;
	k2 : [0..1] init k2_Open;
	t  : [0..2] init t_Closed;

	fs1 : [0..1] init f_no;
	fs  : [0..1] init f_no;
	fk1 : [0..1] init f_no;
	fk2 : [0..1] init f_no;
	ft  : [0..1] init f_no;

	var_t : int init 0;
	var_f : int init 0;

	// commands s1
	[] s1=s1_Start -> 0.5 : (s1'=s1_Start) +
			  0.5 : (s1'=s1_Closed);
	[] s1=s1_Open & fs1!=f_no -> 1.0 : (s1'=s1_Closed);
	[] s1=s1_Closed & fs1=f_no -> 1.0 : (s1'=s1_Open);
	
	// commands t
	[] t=t_Closed & s=s_Closed & (s1=s1_Closed | k1=k1_Closed) -> 1.0 : (t'=t_Countdown);
	[] t=t_Countdown & (s=s_Open | (s1!=s1_Closed & k1=k1_Open)) -> 1.0 : (t'=t_Closed);
	[] t=t_Countdown & var_t<Timeout -> 1.0 : (t'=t_Countdown);
	[] t=t_Countdown & var_t=Timeout & ft=f_no -> 1.0 : (t'=t_Open);
	[] t=t_Countdown & ft=f_yes -> 1.0 : (t'=t_Closed);
	[] t=t_Open & (s=s_Open | (s1!=s1_Closed & k1=k1_Open)) -> 1.0 : (t'=t_Closed);

	// commands var_t
	[] t=t_Closed -> 1.0 : (var_t'=0);
	[] t=t_Countdown & var_t<Timeout -> 1.0 : (var_t'=var_t+1);

	// commands k1
	[] k1=k1_Closed & t=t_Open & fk1=f_no -> 1.0 : (k1'=k1_Open);
	[] k1=k1_Open & t!=t_Open & s1=s1_Closed -> 1.0 : (k1'=k1_Closed);

	// commands k2
	[] k2=k2_Closed & (s=s_Open | (s1!=s1_Closed & k1=k1_Open)) & fk2=f_no -> 1.0 : (k2'=k2_Open);
	[] k2=k2_Open & (s=s_Closed & (s1=s1_Closed | k1=k1_Closed)) -> 1.0 : (k2'=k2_Closed);

	// commands m
	[] m=m_Off & k2=k2_Closed -> 1.0 : (m'=m_On);
	[] m=m_On & k2=k2_Open -> 1.0 : (m'=m_Off);

	// commands s
	[] s=s_Closed & d=d_Filled & fs=f_no -> 1.0 : (s'=s_Open);
	[] s=s_Open & d=d_Empty -> 1.0 : (s'=s_Closed);

	// commands d
	[] d=d_Empty & m=m_On -> 1.0 : (d'=d_Filling);
	[] d=d_Filling & m=m_On & var_f<FillTimeout -> 1.0 : (d'=d_Filling);
	[] d=d_Filling & m=m_On & var_f=FillTimeout -> 1.0 : (d'=d_Filled);
	[] d=d_Filled & m=m_Off -> 1.0 : (d'=d_Draining);
	[] d=d_Draining & m=m_Off & var_f<DrainTime -> 1.0 : (d'=d_Draining);
	[] d=d_Draining & m=m_Off & var_f=DrainTime -> 1.0 : (d'=d_Empty);
	[] d=d_Filled & m=m_On & var_f<RuptureTime -> 1.0 : (d'=d_Filled);
	[] d=d_Filled & m=m_On & var_f=RuptureTime -> 1.0 : (d'=d_Ruptured);

	// commands var_f
	[] d=d_Filling & m=m_On & var_f<FillTimeout -> 1.0 : (var_f'=var_f+1);
	[] d=d_Filled & m=m_On & var_f<RuptureTime -> 1.0 : (var_f'=var_f+1);
	[] d=d_Draining & m=m_Off & var_f<DrainTime -> 1.0 : (var_f'=var_f+1);
	[] d=d_Empty -> 1.0 : (var_f'=0);

	// commands fs1
	[] fs1=f_no -> p_fs1 : (fs1'=f_yes) +
		       1.0 - p_fs1 : (fs1'=f_no);
	[] fs1=f_yes -> p_fs1 : (fs1'=f_yes) +
		       1.0 - p_fs1 : (fs1'=f_no);

	// commands fk1
	[] fk1=f_no -> p_fk1 : (fk1'=f_yes) + 
		       1.0 - p_fk1 : (fk1'=f_no);

	// commands fk2		
	[] fk2=f_no -> p_fk2 : (fk2'=f_yes) + 
		       1.0 - p_fk2 : (fk2'=f_no);

	// commands fs
	[] fs=f_no -> p_fs : (fs'=f_yes) + 
		       1.0 - p_fs : (fs'=f_no);

	// commands ft
	[] ft=f_no -> p_ft : (ft'=f_yes) + 
		       1.0 - p_ft : (ft'=f_no);

endmodule