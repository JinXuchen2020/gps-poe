import { Typography } from 'antd';
import React, { FunctionComponent, useState } from 'react';

export const CollapseText : FunctionComponent<{data: string[], prefix: string}> 
= ({data, prefix}) => {

  const [isCollapsed, setIsCollapsed] = useState(true);
  const [key, setKey] = useState(0);
  const onCollapse = () => {
    setIsCollapsed(true);
    setKey(key + 1)
  };
  
  return (
    <div key={prefix + key} style={{paddingTop: 10}}>
      <Typography.Paragraph ellipsis={
        { 
          rows: 2, 
          expandable: true,
          symbol: '更多',
          onExpand: () => {
            setIsCollapsed(false)
          }
        }
      }>
        {
          data.map((c, index)=>
            <div key={`${prefix}_${index}`}>{c}</div>
          )
        }
        {
          !isCollapsed && <Typography.Link underline={false} onClick={onCollapse}>收起</Typography.Link>
        }
      </Typography.Paragraph>
    </div>    
  );
}
