<?php

$billy = array(
	array("D", 10),
	array("E", 20),
	array("E", 10)
);

echo "<pre>";
print_r($billy);
echo "</pre>";

detect_doubles($billy);

function detect_doubles($arr)
{
	$sjaan = [];
	
	foreach($arr as $elem)
	{
		if(is_array($elem) && (count($elem) > 1))
		{
			$key = $elem[0];
			$count = $elem[1];
			
			if(!array_key_exists($key, $sjaan))
			{
				// We zien deze letter voor het eerst.
				$sjaan[$key] = 0;
			}
			else
			{
				// We hebben deze letter ($count keer) eerder gezien.
			}
			
			$sjaan[$key]++;
		}
		else
		{
			// WTF, het element is niet een array of heeft minder dan twee elementen!
		}
	}
	
	echo "<pre>";
	print_r($sjaan);
	echo "</pre>";
	
	foreach($sjaan as $key => $count)
	{
		if($count > 1)
		{
			echo "Dikke Arie! letter '$key' is meerdere keren ($count) gevonden.";
		}
	}
}
