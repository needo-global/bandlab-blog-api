import { IConstruct } from "constructs";

const getStringValue = (construct: IConstruct, key: string) => {
  return construct.node.tryGetContext(key) as string;
};

export default {
  getStackName: (construct: IConstruct) => {
    return getStringValue(construct, "StackName");
  },
  getStage: (construct: IConstruct) => {
    return getStringValue(construct, "Stage");
  },
  getDockerImageTagForWebApi: (construct: IConstruct) => {
    return getStringValue(construct, "DockerImageTagForWebApi");
  },
};
