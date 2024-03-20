<?php

$harry = array(
	array("A", 10),
	null,
	array(null, 50),
	array("D", 80),
);

echo "<pre>";
print_r($harry);
echo "</pre>";

set_empty($harry, "Pannenkoek!");

function set_empty(&$arr, $replace_with)
{
	$index = 0;
	foreach($arr as $elem)
	{
		if(is_array($elem))
		{
			// Nested, voor 50.
			set_empty($arr[$index], $replace_with);
		}
		else if(is_null($elem) || $elem == "")
		{
			// Eerste level, na A => 10.
			$arr[$index] = $replace_with;
		}
		
		++$index;
	}
	
	return $arr;
}

echo "<pre>";
print_r($harry);
echo "</pre>";

exit;

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
