# Climbing Weather App (caniclimbtoday)

## DISCLAIMER
This application is not perfect nor can it be completely trusted, please use your own judgement at the crag to ensure you do not damage the rock and are safe.

## How to calculate ability to climb?
The most important aspect to whether you can climb outdoors is rock type - the type of rock dictates how all other variables impact climbing conditions. Therefore, I will be maintaining a json file that will initially cover the most common rock types found at British crags.

### Other Variables that impact climbing conditions
It is unwise to climb on rock when it is wet, it not only impacts safety but can lead to breaking the route - _we must avoid this_. I will be taking the average rainfall over the last 3 days as a good indication for how wet a crag may be, we can calculate this using historical data (and future data for predictions) and cross reference this with the rock type to see if a rock is too wet to climb. At first I am going to keep this black and white - you can or can't climb - however I hope to create a sliding scale in the future including wind speeds, sun levels, humidity, etc.

### Formula
My latest calculation method involves a severity scale. The algorithm looks at total rainfall over the last 24 hours, 72 hours, and 7 days - with an appropriate severity scale for each rock type based on total rainfall over the three time periods. The algorithm then takes the highest severity over any period (lean on side of caution) and outputs this as the "severity". 

I have implemented the weather api such that it checks the present day and the next 10 days, this can be used to signify other days in the near future that would be better to climb on. I am still happy to tweak values and potentially include other weather factors but I am happy for now. I believe this new system is a massive improvement on the old.