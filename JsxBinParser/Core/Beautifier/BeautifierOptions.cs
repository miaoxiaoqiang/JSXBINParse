namespace Jsxbeautifier
{
    internal sealed class BeautifierOptions
    {
    	public uint IndentSize { get; set; }
    
    	public char IndentChar { get; set; }
    
    	public bool IndentWithTabs { get; set; }
    
    	public bool PreserveNewlines { get; set; }
    
    	public float MaxPreserveNewlines { get; set; }
    
    	public bool JslintHappy { get; set; }
    
    	public BraceStyle BraceStyle { get; set; }
    
    	public bool KeepArrayIndentation { get; set; }
    
    	public bool KeepFunctionIndentation { get; set; }
    
    	public bool EvalCode { get; set; }
    
    	public bool BreakChainedMethods { get; set; }
    
    	public BeautifierOptions()
    	{
    		IndentSize = 4u;
    		IndentChar = ' ';
    		IndentWithTabs = false;
    		PreserveNewlines = true;
    		MaxPreserveNewlines = 10f;
    		JslintHappy = false;
    		BraceStyle = BraceStyle.Collapse;
    		KeepArrayIndentation = false;
    		KeepFunctionIndentation = false;
    		EvalCode = false;
    		BreakChainedMethods = false;
    	}
    
    	public static BeautifierOptions DefaultOptions()
    	{
    		return new BeautifierOptions();
    	}
    }
}